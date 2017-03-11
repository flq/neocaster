using System.Collections.Generic;
using Neo4j.Driver.V1;

namespace NeoCaster.Tests.DryRun
{
    public class DryRunSession : ISession
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public IStatementResult Run(string statement, IDictionary<string, object> parameters = null)
        {
            throw new System.NotImplementedException();
        }

        public IStatementResult Run(Statement statement)
        {
            throw new System.NotImplementedException();
        }

        public IStatementResult Run(string statement, object parameters)
        {
            throw new System.NotImplementedException();
        }

        public ITransaction BeginTransaction(string bookmark = null)
        {
            throw new System.NotImplementedException();
        }

        public string LastBookmark { get; }
    }
}