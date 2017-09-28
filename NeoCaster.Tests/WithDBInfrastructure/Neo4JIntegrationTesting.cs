using System;
using JetBrains.Annotations;
using Neo4j.Driver.V1;
using Xunit;

namespace NeoCaster.Tests.WithDBInfrastructure
{
    [CollectionDefinition(nameof(IntegrationRun))]
    public class IntegrationRun : ICollectionFixture<Neo4JIntegrationTesting>, IDisposable
    {
        public void Dispose() => Neo4JIntegrationTesting.Dispose();
    }

    [UsedImplicitly]
    public class Neo4JIntegrationTesting
    {
        private static readonly Lazy<IDriver> _driver = new Lazy<IDriver>(() =>
        {
            var d = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.None);
            try
            {
                using (var s = d.Session())
                    s.Run("MATCH (n) DETACH DELETE n");
                return d;
            }
            catch(AuthenticationException)
            {
                FailureMessage = "Auth failed. Integration Tests assume that you run the db with no auth";
                return null;
            }
            catch (ServiceUnavailableException)
            {
                FailureMessage = "It looks like there is no neo4j DB running at localhost:7687";
                return null;
            }
        });

        private static string _failureMessage;

        public static string FailureMessage
        {
            get
            {
                if (!_driver.IsValueCreated)
                {
                    var _ = _driver.Value;
                }
                return _failureMessage;
            }
            private set => _failureMessage = value;
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

        public IStatementResult RunStatement(string cypher, object parameters)
        {
            IStatementResult returnValue = null;
            WithSession(s =>
            {
                returnValue = s.Run(cypher, parameters);
            });
            return returnValue;
        }

        private void WithSession(Action<ISession> sessionAction)
        {
            using (var session = GetDriver().Session())
            {
                sessionAction(session);
            }
        }

        private static IDriver GetDriver() {
            return _driver.Value;
        }

        public static void Dispose()
        {
            _driver?.Value.Dispose();
        }
    }

    public class RequireNeoFactAttribute : FactAttribute
    {
        public RequireNeoFactAttribute()
        {
            if (!string.IsNullOrEmpty(Neo4JIntegrationTesting.FailureMessage))
            {
                Skip = Neo4JIntegrationTesting.FailureMessage;
            }
        }
    }
}