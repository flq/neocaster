using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neo4j.Driver.V1;
using Sigil;

namespace NeoCaster
{
    internal class PreparedStatement<TReturn> : PreparedStatement
    {
        private MapStrategy _mapStrategy;

        public PreparedStatement(string cypher, object parameters = null) : base(typeof(TReturn), cypher, parameters)
        {
        }

        public TReturn Map(IRecord record)
        {
            if (_mapStrategy == null)
            {
                lock (Locker)
                {
                    if (_mapStrategy == null)
                        _mapStrategy = new MapStrategy(record);
                }
            }

            return _mapStrategy.Map(record);
        }
        
        internal class MapStrategy
        {
            private readonly Func<IRecord, TReturn> _mapMethod;

            public MapStrategy(IRecord record)
            {
                if (record.ContainsOnlyPrimitives())
                {
                    var loader = DictionaryLoad.DirectlyFromRecord();
                    _mapMethod = BuildMappingMethod(
                        from p in typeof(TReturn).GetProperties(ObjectToDictionary.BindingFlagsForDTO)
                        let neoName = Config.DotnetToNeoPropNames(p.Name)
                        where p.CanWrite
                        select new PropertyMap { Loader = loader, Property = p, RecordKey = neoName });
                }
                else if (record.ContainsOnlySingleNode())
                {
                    var nodeKey = record.Keys.First();
                    var loader = DictionaryLoad.FromEmbeddedNode(nodeKey);
                    _mapMethod = BuildMappingMethod(
                        from p in typeof(TReturn).GetProperties(ObjectToDictionary.BindingFlagsForDTO)
                        let neoName = Config.DotnetToNeoPropNames(p.Name)
                        where p.CanWrite
                        select new PropertyMap { Loader = loader, Property = p, RecordKey = neoName });
                }
            }

            public TReturn Map(IRecord record)
            {
                return _mapMethod(record);
            }

            private Func<IRecord,TReturn> BuildMappingMethod(IEnumerable<PropertyMap> propertyMaps)
            {
                var emit = Emit<Func<IRecord, TReturn>>.NewDynamicMethod($"MapTo_{typeof(TReturn).Name}_{Guid.NewGuid().ToBase64Guid()}");
                var ret = emit.DeclareLocal(typeof(TReturn));
                var v = emit.DeclareLocal(typeof(object));
                var check = emit.DeclareLocal(typeof(bool));
                var valueDict = emit.DeclareLocal(typeof(IReadOnlyDictionary<string, object>));
                emit.NewObject(typeof(TReturn));
                emit.StoreLocal(ret);

                int labelCounter = 0;
                DictionaryLoad currentLoader = null;

                foreach (var pm in propertyMaps)
                {
                    if (!ReferenceEquals(currentLoader, pm.Loader))
                    {
                        // valueDict = record.Values || ((INode)record["v"]).Properties || ...
                        currentLoader = pm.Loader;
                        currentLoader.EmitLoad(emit, valueDict);
                    }

                    // check = dict.TryGetValue(recordKey, out v)
                    // if (check)
                    //   ret.Property = (cast)v;
                    var lbl = emit.DefineLabel("lbl_" + labelCounter);
                    emit.LoadLocal(valueDict);
                    emit.LoadConstant(pm.RecordKey);
                    emit.LoadLocalAddress(v);
                    emit.CallVirtual(TryGetValue);
                    emit.AsShorthand().Ldc(0); // <-- Copied from looking at IL
                    emit.AsShorthand().Ceq();  // it seems to turn around the true val
                    emit.StoreLocal(check);    // and jump to the label
                    emit.LoadLocal(check);
                    emit.BranchIfTrue(lbl);

                    emit.LoadLocal(ret);
                    emit.LoadLocal(v);

                    if (pm.Property.PropertyType.GetTypeInfo().IsValueType)
                    {
                        emit.UnboxAny(pm.Property.PropertyType);
                    }
                    else
                        emit.CastClass(pm.Property.PropertyType);
                    emit.CallVirtual(pm.Property.SetMethod);
                    emit.MarkLabel(lbl);
                    labelCounter++;
                }

                emit.LoadLocal(ret);
                emit.Return();
                return emit.CreateDelegate();
            }
        }

        internal class PropertyMap
        {
            public PropertyInfo Property { get; set; }
            public string RecordKey { get; set; }
            public DictionaryLoad Loader { get; set; }
        }

        internal class DictionaryLoad
        {
            private readonly Action<Emit<Func<IRecord, TReturn>>,Local> _emitter;

            private DictionaryLoad(Action<Emit<Func<IRecord, TReturn>>, Local> emitter)
            {
                _emitter = emitter;
            }

            /// <summary>
            /// The node itself contains properties that aren't nodes or relationships
            /// </summary>
            public static DictionaryLoad DirectlyFromRecord()
            {
                return new DictionaryLoad((emit, local) =>
                {
                    emit.LoadArgument(0);
                    emit.CallVirtual(RecordValuesAccessor);
                    emit.StoreLocal(local);
                });
            }

            /// <summary>
            /// All trivial properties are extracted from a single node contained in the record.
            /// </summary>
            public static DictionaryLoad FromEmbeddedNode(string key)
            {
                return new DictionaryLoad((emit, local) =>
                {
                    emit.LoadArgument(0);
                    emit.CallVirtual(RecordValuesAccessor);
                    emit.LoadConstant(key);
                    emit.CallVirtual(GetDictionaryItem);
                    emit.CastClass<IEntity>();
                    emit.CallVirtual(NodePropertiesAccessor);
                    emit.StoreLocal(local);
                });
            }

            /// <summary>
            /// Ensure that the correct dictionary is put into the local dictionary variable
            /// such that accesses work from there
            /// </summary>
            public void EmitLoad(Emit<Func<IRecord, TReturn>> emit, Local dictionary)
            {
                _emitter(emit, dictionary);
            }
        }
    }

    internal class PreparedStatement : IEquatable<PreparedStatement>
    {
        protected static readonly object Locker = new object();
        protected static readonly MethodInfo RecordValuesAccessor;
        protected static readonly MethodInfo GetDictionaryItem;
        protected static readonly MethodInfo TryGetValue;
        protected static readonly MethodInfo NodePropertiesAccessor;

        private readonly Type _returnType;
        private readonly string _cypher;
        private readonly object _parameters;

        static PreparedStatement()
        {
            RecordValuesAccessor = typeof(IRecord).GetProperty(nameof(IRecord.Values)).GetMethod;
            NodePropertiesAccessor = typeof(IEntity).GetProperty(nameof(IEntity.Properties)).GetMethod;
            GetDictionaryItem = typeof(IReadOnlyDictionary<string, object>)
                .GetProperties()
                .First(p => p.GetIndexParameters().Length > 0 &&
                            p.GetIndexParameters()[0].ParameterType == typeof(string)).GetMethod;
            TryGetValue = typeof(IReadOnlyDictionary<string, object>).GetMethod("TryGetValue");
        }

        public PreparedStatement(Type returnType, string cypher, object parameters = null)
        {
            _returnType = returnType;
            _cypher = cypher;
            _parameters = parameters;
        }

        public bool Equals(PreparedStatement other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_cypher, other._cypher)
                   && _returnType == other._returnType
                   && _parameters?.GetType() == other._parameters?.GetType();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PreparedStatement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _returnType.GetHashCode();
                hashCode = (hashCode * 397) ^ _cypher.GetHashCode();
                hashCode = (hashCode * 397) ^ (_parameters?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}