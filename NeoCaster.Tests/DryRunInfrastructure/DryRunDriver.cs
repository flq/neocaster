using System;
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver.V1;

namespace NeoCaster.Tests.DryRunInfrastructure
{
    /// <inheritdoc />
    /// <summary>
    /// check docs
    /// https://github.com/neo4j/neo4j-dotnet-driver/wiki/1.2-Driver-documentation
    /// bookmark: Similar functionality to etag.
    /// </summary>
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

        public ISession Session(string bookmark)
        {
            CheckDisposed();
            return new DryRunSession(bookmark);
        }

        /// <summary>
        /// Our DryRunSession won't be able to recognize wether the user is attempting a mutation
        /// Hence we will not evaluate the access mode
        /// </summary>
        public ISession Session(AccessMode defaultMode, string bookmark)
        {
            CheckDisposed();
            return new DryRunSession(bookmark);
        }

        public ISession Session(AccessMode defaultMode, IEnumerable<string> bookmarks)
        {
            CheckDisposed();
            return new DryRunSession(bookmarks.FirstOrDefault());
        }

        public ISession Session(IEnumerable<string> bookmarks)
        {
            CheckDisposed();
            return new DryRunSession(bookmarks.FirstOrDefault());
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