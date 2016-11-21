using System.Collections.Generic;
using Neo4j.Driver.V1;

namespace NeoCaster
{
    public static class NeoMapping
    {
        public static IEnumerable<T> Run<T>(this IStatementRunner runner, string cypher, object parameters) 
        {
            var result = runner.Run(cypher, parameters.ToDictionary());
            return null;
        }
    }
}
