using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver.V1;
using Newtonsoft.Json.Linq;

namespace NeoCaster.Tests.DryRun
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

        public IReadOnlyList<string> Keys
        {
            get { throw new NotSupportedException("Not for now"); }
        }

        public IResultSummary Summary { get { throw new NotSupportedException("Not for now"); } }

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
        private readonly IReadOnlyDictionary<string, object> _contents;

        public DryNode(JObject jObject)
        {
            IDictionary<string, JToken> tmp = jObject;
            _contents = tmp.ToDictionary(k => k.Key, v => v.Value.ConvertToNetType());
            Id = (long)_contents["$id"];
            Labels = ((object[])_contents["$labels"]).Cast<string>().ToList();
        }

        public object this[string key] => _contents.ContainsKey(key) ? _contents[key] : null;

        public IReadOnlyDictionary<string, object> Properties => _contents;

        public long Id { get; }

        public bool Equals(INode other) => Id.Equals(other.Id);

        public IReadOnlyList<string> Labels { get; }
    }
}