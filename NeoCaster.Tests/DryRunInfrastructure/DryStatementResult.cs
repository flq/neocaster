using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver.V1;
using Newtonsoft.Json.Linq;

namespace NeoCaster.Tests.DryRunInfrastructure
{
    internal class DryStatementResult : IStatementResult
    {
        private readonly JArray _skeleton;

        private IList<JToken> Accessor => _skeleton;


        public DryStatementResult(JArray skeleton)
        {
            _skeleton = skeleton;
        }

        public IEnumerator<IRecord> GetEnumerator()
        {
            return Accessor.Select(ConvertToRecord).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IRecord Peek()
        {
            throw new NotSupportedException("Not for now");
        }

        public IResultSummary Consume()
        {
            throw new NotSupportedException("Not for now");
        }

        public IReadOnlyList<string> Keys => throw new NotSupportedException("Not for now");

        public IResultSummary Summary => throw new NotSupportedException("Not for now");

        private IRecord ConvertToRecord(JToken arg)
        {
            return new DryRecord(arg);
        }
    }

    internal class DryRecord : IRecord
    {

        private readonly JObject _contents;

        private IDictionary<string, JToken> Accessor => _contents;

        public DryRecord(JToken contents)
        {
            _contents = (JObject)contents; // A record can always be mapped into an object structure
        }

        object IRecord.this[int index] => Accessor.Skip(index).FirstOrDefault().Value.ConvertToNetType();

        object IRecord.this[string key] => _contents[key].ConvertToNetType();

        public IReadOnlyDictionary<string, object> Values
        {
            get { return Accessor.ToDictionary(k => k.Key, v => v.Value.ConvertToNetType()); }
        }

        public IReadOnlyList<string> Keys => new List<string>(Accessor.Keys);

    }

    internal class DryNode : INode
    {
        public DryNode(JObject jObject)
        {
            IDictionary<string, JToken> tmp = jObject;
            Properties = tmp.ToDictionary(k => k.Key, v => v.Value.ConvertToNetType());
            Id = (long)Properties["$id"];
            Labels = ((object[])Properties["$labels"]).Cast<string>().ToList();
        }

        public object this[string key] => Properties.ContainsKey(key) ? Properties[key] : null;

        public IReadOnlyDictionary<string, object> Properties { get; }

        public long Id { get; }

        public bool Equals(INode other) => Id.Equals(other.Id);

        public IReadOnlyList<string> Labels { get; }
    }

    internal class DryRelationship : IRelationship
    {

        public DryRelationship(JObject jObject)
        {
            IDictionary<string, JToken> tmp = jObject;
            Properties = tmp.ToDictionary(k => k.Key, v => v.Value.ConvertToNetType());
            Id = (long)Properties["$id"];
            Type = (string)Properties["$relType"];
            StartNodeId = (long)Properties["$startNodeId"];
            EndNodeId = (long)Properties["$endNodeId"];
        }

        public object this[string key] => Properties.ContainsKey(key) ? Properties[key] : null;
        public long Id { get; }
        public IReadOnlyDictionary<string, object> Properties { get; }

        public bool Equals(IRelationship other) => Id.Equals(other.Id);

        public string Type { get; }
        public long StartNodeId { get; }
        public long EndNodeId { get; }
    }
}