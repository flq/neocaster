using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sigil;

namespace NeoCaster
{
    public static class ObjectToDictionary
    {
        public const BindingFlags BindingFlagsForDTO = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        static readonly ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>> ConvertersCache =
            new ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>>();
        static readonly Type DictType = typeof(Dictionary<string, object>);
        static readonly MethodInfo AddMethod = DictType.GetRuntimeMethod("Add", new[] {typeof(string), typeof(object)});


        /// <summary>
        /// Clear the cache holding the converters that convert an anonymous object instance to a dictionary instance
        /// suitable to be passed as parameter object to a cypher statement.
        /// </summary>
        public static void Clear()
        {
            ConvertersCache.Clear();
        }


        /// <summary>
        /// Convert an object to a dictionary by considering its public instance fields and writing it into a
        /// dictionary with Key = PropertyName and Value = value of property.
        /// Note that the read/write is optimized and cached. If you want to get rid of the cache, You can call the
        /// <see cref="Clear"/>-method.
        /// </summary>
        /// <param name="parameterObject">The object to translate.</param>
        /// <returns>A dictionary with the extracted contents</returns>
        public static IDictionary<string, object> Convert(this object parameterObject)
        {
            Require.NotNull(parameterObject, nameof(parameterObject));
            var f = ConvertersCache.GetOrAdd(parameterObject.GetType(), ConstructFactory);
            return f(parameterObject);
        }

        private static Func<object, Dictionary<string, object>> ConstructFactory(Type parameterObjectType)
        {
            var emit = Emit<Func<object, Dictionary<string, object>>>.NewDynamicMethod("Convert" + Guid.NewGuid());

            // var dict = new Dictionary();
            // var input = (ParameterObjectType) argument 0;
            // -->
            var theDict = emit.DeclareLocal(DictType);
            var theInput = emit.DeclareLocal(parameterObjectType);
            emit.NewObject<Dictionary<string, object>>();
            emit.StoreLocal(theDict);
            emit.LoadArgument(0);
            emit.CastClass(parameterObjectType);
            emit.StoreLocal(theInput);

            foreach (var property in parameterObjectType.GetTypeInfo()
                .GetProperties(BindingFlagsForDTO)
                .Where(info => info.CanRead))
            {
                // dict, property name, property value (box if value type) -> Add
                // -->
                emit.LoadLocal(theDict);
                emit.LoadConstant(property.Name);
                emit.LoadLocal(theInput);
                emit.CallVirtual(property.GetGetMethod());
                if (property.PropertyType.GetTypeInfo().IsValueType)
                    emit.Box(property.PropertyType); // Because it goes into an object box on the dictionary
                emit.CallVirtual(AddMethod);
            }
            // return dict;
            // -->
            emit.LoadLocal(theDict);
            emit.Return();
            return emit.CreateDelegate();
        }
    }
}