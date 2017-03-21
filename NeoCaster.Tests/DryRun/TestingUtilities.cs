using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo4j.Driver.V1;
using Newtonsoft.Json.Linq;

namespace NeoCaster.Tests.DryRun
{
    public static class TestingUtilities
    {
        /// <summary>
        /// For debugging purposes, use the StatementResult render method to write it to a file.
        /// </summary>
        public static void WriteToFile(this IStatementResult result, string path)
        {
            using (var f = File.OpenWrite(path))
            using (var w = new StreamWriter(f))
            {
                result.Render(w);
            }
        }

        public static object ConvertToNetType(this JToken token)
        {
            if (token is JObject)
                return new DryNode((JObject)token);
            if (token is JArray)
                return ((IList<JToken>) token).Select(t => t.ConvertToNetType()).ToArray();
            if (token is JValue)
                return ((JValue) token).Value;
            throw new ArgumentException($"{token.GetType().Name}? What to do about it?");
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