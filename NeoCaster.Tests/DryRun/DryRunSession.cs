using System;
using System.Collections.Generic;
using Neo4j.Driver.V1;

namespace NeoCaster.Tests.DryRun
{
    public class DryRunSession : ISession
    {
        private readonly Func<string, IStatementResult> _resultFac;

        private bool _disposed;
        private int _bookmark;

        public DryRunSession() { }

        public DryRunSession(Func<string,IStatementResult> resultFac)
        {
            _resultFac = resultFac;
        }

        public void Dispose()
        {
            _disposed = true;
        }

        public IStatementResult Run(string statement, IDictionary<string, object> parameters = null)
        {
            if (parameters == null && _resultFac != null)
                return _resultFac(statement);
            throw new NotSupportedException();
        }

        public IStatementResult Run(Statement statement)
        {
            return null;
        }

        public IStatementResult Run(string statement, object parameters)
        {
            return null;
        }

        public ITransaction BeginTransaction(string bookmark = null)
        {
            return null;
        }

        public string LastBookmark => _bookmark.ToString();
    }
}