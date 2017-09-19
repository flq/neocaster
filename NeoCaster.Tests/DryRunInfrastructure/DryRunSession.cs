using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver.V1;

namespace NeoCaster.Tests.DryRunInfrastructure
{
    public class DryRunSession : ISession
    {
        private readonly Func<string, IStatementResult> _resultFac;

        private bool _disposed;
        private string _bookmark;

        public DryRunSession(string bookmark = null)
        {
            _bookmark = bookmark;
        }

        /// <summary>
        /// Provide a factory that, given a statement, will produce some Statement Result
        /// </summary>
        /// <param name="resultFac">A statement result factory. The string is some statement provided through a call to "Run"</param>
        public DryRunSession(Func<string,IStatementResult> resultFac)
        {
            _resultFac = resultFac;
        }

        public void Dispose()
        {
            _disposed = true;
        }

        public Task<IStatementResultCursor> RunAsync(string statement, object parameters)
        {
            throw new NotImplementedException();
        }

        public IStatementResult Run(string statement, IDictionary<string, object> parameters)
        {
            return Run(statement);
        }

        public IStatementResult Run(string statement)
        {
            return _resultFac?.Invoke(statement) ?? throw new NotSupportedException();
        }

        public Task<IStatementResultCursor> RunAsync(string statement, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public IStatementResult Run(Statement statement)
        {
            return null;
        }

        public Task<IStatementResultCursor> RunAsync(Statement statement)
        {
            throw new NotImplementedException();
        }

        public Task<IStatementResultCursor> RunAsync(string statement)
        {
            throw new NotImplementedException();
        }

        public IStatementResult Run(string statement, object parameters)
        {
            return null;
        }

        public ITransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public ITransaction BeginTransaction(string bookmark = null)
        {
            _bookmark = bookmark;
            return null;
        }

        public Task<ITransaction> BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public T ReadTransaction<T>(Func<ITransaction, T> work)
        {
            throw new NotImplementedException();
        }

        public Task<T> ReadTransactionAsync<T>(Func<ITransaction, Task<T>> work)
        {
            throw new NotImplementedException();
        }

        public void ReadTransaction(Action<ITransaction> work)
        {
            throw new NotImplementedException();
        }

        public Task ReadTransactionAsync(Func<ITransaction, Task> work)
        {
            throw new NotImplementedException();
        }

        public T WriteTransaction<T>(Func<ITransaction, T> work)
        {
            throw new NotImplementedException();
        }

        public Task<T> WriteTransactionAsync<T>(Func<ITransaction, Task<T>> work)
        {
            throw new NotImplementedException();
        }

        public void WriteTransaction(Action<ITransaction> work)
        {
            throw new NotImplementedException();
        }

        public Task WriteTransactionAsync(Func<ITransaction, Task> work)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public string LastBookmark => _bookmark;
    }
}