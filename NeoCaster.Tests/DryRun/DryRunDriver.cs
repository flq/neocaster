using System;
using Neo4j.Driver.V1;

namespace NeoCaster.Tests.DryRun
{
    public class DryRunDriver : IDriver
    {
        private bool _disposed;

        public ISession Session()
        {
            CheckDisposed();
            return new DryRunSession();
        }

        public ISession Session(AccessMode mode)
        {
            CheckDisposed();
            return new DryRunSession();
        }

        public void Dispose()
        {
            _disposed = true;
        }

        public Uri Uri { get; } = new Uri("bolt://etcpp");

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("Session");
        }
    }
}