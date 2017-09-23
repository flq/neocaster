using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NeoCaster.Tests.DryRunInfrastructure
{
    public static class TestingUtilities
    {

        public static object ConvertToNetType(this JToken token)
        {
            switch (token)
            {
                case JObject j:
                    switch (j["$type"].ToString())
                    {
                        case "node": return new DryNode(j);
                        case "relationship": return new DryRelationship(j);
                    }
                    break;
                case JArray a:
                    return a.Select(t => t.ConvertToNetType()).ToArray();
                case JValue v:
                    return v.Value;
            }
            throw new ArgumentException($"Could not convert JToken {token.GetType().Name} to Net Type - What to do about it?");
        }
    }

    public abstract class AbstractClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            return TestData();
        }

        protected abstract IEnumerator<object[]> TestData();

        protected object[] Data(params object[] data)
        {
            return data;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}