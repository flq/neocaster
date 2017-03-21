using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo4j.Driver.V1;

namespace NeoCaster
{
    public static class RenderStatementResultExtension
    {
        const string indentInsideRecord = "    ";

        /// <summary>
        /// Gives you a JSON-representation of the Statement result presented.
        /// </summary>
        public static void Render(this IEnumerable<IRecord> result, TextWriter writer)
        {
            writer.WriteLine("[");
            foreach(var r in result)
            {
                RenderRecord(r, writer);
            }
            writer.WriteLine("]");
            writer.Flush();
        }

        private static void RenderRecord(IRecord record, TextWriter writer)
        {
            writer.WriteLine("{");
            foreach(var r in record.Values)
            {
                writer.Write($"  {r.Key.Quotify()}: ");
                RenderValue(r.Value, writer);
            }
            writer.WriteLine("},");
        }

        private static void RenderValue(object val, TextWriter writer)
        {
            if (val is INode)
            {
                var n = val.As<INode>();
                RenderNode(n, writer);
            }
            else if (val is long)
            {
                writer.WriteLine(val + ", ");
            }
            else if (val is bool)
            {
                writer.WriteLine(val.ToString().ToLowerInvariant() + ", ");
            }
            else if (val is string)
            {
                writer.WriteLine($"{val.ToString().Quotify()}, ");
            }
            else if (val == null)
            {
                writer.WriteLine("null, ");
            }
            else
            {
                writer.Write($"\"{val.GetType()?.Name ?? "NULL?!"}-{val}\", ");
            }
        }

        private static void RenderNode(INode node, TextWriter writer)
        {
            writer.WriteLine("{");
            writer.WriteLine($"{indentInsideRecord}{"$id".Quotify()}: {node.Id},");
            writer.WriteLine($"{indentInsideRecord}{"$type".Quotify()}: {"node".Quotify()},");
            writer.WriteLine($"{indentInsideRecord}{"$labels".Quotify()}: [{string.Join(",", node.Labels.Select(l => l.Quotify()))}],");
            foreach (var kv in node.Properties)
            {
                writer.WriteLine($"{indentInsideRecord}\"{kv.Key}\": {RenderPrimitive(kv.Value)}, ");
            }
            writer.WriteLine("  },");
        }

        private static string RenderPrimitive(object p)
        {
            if (p is string)
                return p.ToString().Quotify();
            if (p is bool)
                return p.ToString().ToLowerInvariant();
            return p.ToString();
        }

        private static string Quotify(this string text)
        {
            return $"\"{text}\"";
        }
    }
}