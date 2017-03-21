using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Neo4j.Driver.V1;

namespace NeoCaster
{
    public static class NeoCasting
    {
        private static readonly ConcurrentDictionary<int, PreparedStatement> PreparedStatements =
            new ConcurrentDictionary<int, PreparedStatement>();

        public static int NumberOfPreparedStatements => PreparedStatements.Count;

        public static IEnumerable<T> Query<T>(this IStatementRunner session, string cypher)
        {
            var forLookup = new PreparedStatement<T>(cypher);
            var ps = (PreparedStatement<T>)PreparedStatements.GetOrAdd(forLookup.GetHashCode(), hash => forLookup);

            foreach (var record in session.Run(cypher))
            {
                yield return ps.Map(record);
            }
        }
    }
}