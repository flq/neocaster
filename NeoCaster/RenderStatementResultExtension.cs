using System.IO;
using System.Linq;
using Neo4j.Driver.V1;

namespace NeoCaster
{
    public static class RenderStatementResultExtension
    {
        /// <summary>
        /// Gives you a JSON-representation of the Statement result presented.
        /// </summary>
        public static void Render(this IStatementResult result, TextWriter writer)
        {
            writer.WriteLine("{");
            foreach(var r in result)
            {
                RenderRecord(r, writer);
            }
            writer.WriteLine("}");
        }

        private static void RenderRecord(IRecord record, TextWriter writer)
        {
            foreach(var r in record.Values)
            {
                writer.Write($"  {r.Key.Quotify()}: ");
                RenderValue(r.Value, writer);
            }
        }

        private static void RenderValue(object val, TextWriter writer)
        {
            if (val is INode)
            {
                var n = val.As<INode>();
                RenderNode(n, writer);
            }
        }

        private static void RenderNode(INode node, TextWriter writer)
        {
            const string indent = "    ";
            writer.WriteLine("{");
            writer.WriteLine($"{indent}{"labels".Quotify()}: [{string.Join(",", node.Labels.Select(l => l.Quotify()))}],");
            foreach (var kv in node.Properties)
            {
                writer.WriteLine($"{indent}\"{kv.Key}\": {RenderPrimitive(kv.Value)}, ");
            }
            writer.WriteLine("  }");
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