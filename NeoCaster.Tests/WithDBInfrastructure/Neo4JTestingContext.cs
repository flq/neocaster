using System;
using System.Linq;
using Neo4j.Driver.V1;
using Xunit;

namespace NeoCaster.Tests.WithDBInfrastructure
{
    [CollectionDefinition(nameof(WetRun))]
    public class WetRun : ICollectionFixture<Neo4JTestingContext>
    {
    }

    public class Neo4JTestingContext : IDisposable
    {
        private IDriver _driver;

        public Neo4JTestingContext()
        {
            try
            {
                WithSession(s => s.Run("MATCH (n) DETACH DELETE n"));
            }
            catch (AuthenticationException)
            {
                Assert.True(false, "Auth failed. Tests assume that you run the db with no auth");
            }
            catch (ServiceUnavailableException x)
            {
                throw new InvalidOperationException("It looks like there is no neo4j DB running at localhost:7687", x);
            }
        }

        public IDriver GetDriver() {
           return _driver ?? (_driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.None));
        }

        public void WithSession(Action<ISession> sessionAction)
        {
            using (var session = GetDriver().Session())
            {
                sessionAction(session);
            }
        }

        public T RunScenario<T>() where T : IScenario, new()
        {
            var scenario = new T();
            WithSession(s => scenario.Execute(s));
            return scenario;
        }

        public IStatementResult RunStatement(string cypher)
        {
            IStatementResult returnValue = null;
            WithSession(s =>
            {
                returnValue = s.Run(cypher);
            });
            return returnValue;
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }

    public class ConnectionUnavailableException : Exception
    {
        public ConnectionUnavailableException(Exception inner)
            : base("Neo4J is unavailable, make sure it is running and that your test instance does not need authentication.", inner)
        {

        }
    }
}