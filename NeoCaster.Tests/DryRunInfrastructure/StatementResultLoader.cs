using System.Reflection;
using Neo4j.Driver.V1;

namespace NeoCaster.Tests.DryRunInfrastructure
{
    public static class StatementResultLoader
    {
        /// <summary>
        /// Creates a <see cref="DryStatementResult"/> from a file embedded under "StatementResults"
        /// </summary>
        /// <param name="name">The name of the file without extension.</param>
        /// <returns>A statement result</returns>
        public static IStatementResult LoadFromEmbedded(string name)
        {
            var s = typeof(StatementResultLoader).GetTypeInfo()
                .Assembly.GetManifestResourceStream($"NeoCaster.Tests.StatementResults.{name}.json");
            return new DryStatementResult(StatementResultStorage.GetContentsFromStream(s));
        }

        public static string ShowEmbeddedStuff()
        {
            return string.Join(", ", typeof(StatementResultLoader).GetTypeInfo()
                .Assembly.GetManifestResourceNames());
        }
    }
}