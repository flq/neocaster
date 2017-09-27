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
                writer.RenderRecord(r);
            }
            writer.WriteLine("]");
            writer.Flush();
        }

        private static void RenderRecord(this TextWriter writer, IRecord record)
        {
            writer.WriteLine("{");
            foreach(var r in record.Values)
            {
                writer.Write($"  {r.Key.Quotify()}: ");
                writer.RenderValue(r.Value);
            }
            writer.WriteLine("},");
        }

        private static void RenderValue(this TextWriter writer, object val)
        {
            switch (val)
            {
                case INode n:
                    RenderNode(writer, n);
                    break;
                case IRelationship r:
                    RenderRelationship(writer, r);
                    break;
                case IList<object> l:
                    writer.Write("[");
                    foreach (var o in l) writer.RenderValue(o);
                    writer.Write("]");
                    break;
                case long _:
                    writer.WriteLine($"{val}, ");
                    break;
                case bool _:
                    writer.WriteLine($"{val.ToString().ToLowerInvariant()}, ");
                    break;
                case string _:
                    writer.WriteLine($"{val.ToString().Quotify()}, ");
                    break;
                case null:
                    writer.WriteLine("null, ");
                    break;
                default:
                    writer.Write($"\"{val.GetType()?.Name ?? "NULL?!"}-{val}\", ");
                    break;
            }
        }

        private static void RenderRelationship(TextWriter writer, IRelationship r)
        {
            writer.WriteLine("{");
            RenderId(writer, r);
            writer.WriteLine($"{indentInsideRecord}{"$type".Quotify()}: {"relationship".Quotify()},");
            writer.WriteLine($"{indentInsideRecord}{"$relType".Quotify()}: {r.Type.Quotify()},");
            writer.WriteLine($"{indentInsideRecord}{"$startNodeId".Quotify()}: {r.StartNodeId},");
            writer.WriteLine($"{indentInsideRecord}{"$endNodeId".Quotify()}: {r.EndNodeId},");
            RenderProperties(writer, r.Properties);
            writer.WriteLine("  },");
        }

        private static void RenderNode(TextWriter writer, INode node)
        {
            writer.WriteLine("{");
            writer.RenderId(node);
            writer.WriteLine($"{indentInsideRecord}{"$type".Quotify()}: {"node".Quotify()},");
            writer.WriteLine($"{indentInsideRecord}{"$labels".Quotify()}: [{string.Join(",", node.Labels.Select(l => l.Quotify()))}],");
            RenderProperties(writer, node.Properties);
            writer.WriteLine("  },");
        }

        private static void RenderId(this TextWriter writer, IEntity node)
        {
            writer.WriteLine($"{indentInsideRecord}{"$id".Quotify()}: {node.Id},");
        }

        private static void RenderProperties(TextWriter writer, IReadOnlyDictionary<string, object> props)
        {
            foreach (var kv in props)
            {
                writer.WriteLine($"{indentInsideRecord}\"{kv.Key}\": {RenderPrimitive(kv.Value)}, ");
            }
        }

        private static string RenderPrimitive(object p)
        {
            if (p is string)
                return p.ToString().Quotify();
            if (p is long)
                return p.ToString();
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