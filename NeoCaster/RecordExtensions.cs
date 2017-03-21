using System;
using System.Linq;
using Neo4j.Driver.V1;

namespace NeoCaster
{
    internal static class RecordExtensions
    {
        public static bool ContainsOnlyPrimitives(this IRecord record)
        {
            return record.Values.Select(kv => kv.Value).All(o => o.IsNeoPrimitive());
        }

        public static bool ContainsOnlySingleNode(this IRecord record)
        {
            return record.Values.Count == 1 && record.Values.First().Value is INode;
        }

        public static bool IsNeoPrimitive(this object o)
        {
            return o != null && o.GetType().IsNeoPrimitive();
        }

        public static bool IsNeoPrimitive(this Type t)
        {
            if (t.IsArray)
            {
                var elementType = t.GetElementType();
                return elementType.IsNonArrayPrimitive();
            }
            return t.IsNonArrayPrimitive();
        }

        private static bool IsNonArrayPrimitive(this Type type)
        {
            return type == typeof(bool)
                   || type == typeof(int)
                   || type == typeof(long)
                   || type == typeof(string)
                   || type == typeof(char);
        }
    }
}