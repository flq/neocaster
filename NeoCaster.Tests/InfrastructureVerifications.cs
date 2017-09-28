using System.Linq;
using Neo4j.Driver.V1;
using NeoCaster.Tests.DryRunInfrastructure;
using NeoCaster.Tests.WithDBInfrastructure;
using Shouldly;
using Xunit;

namespace NeoCaster.Tests
{
    [Collection(nameof(IntegrationRun))]
    public class InfrastructureVerifications
    {
        private readonly Neo4JIntegrationTesting _ctx;

        public InfrastructureVerifications(Neo4JIntegrationTesting ctx)
        {
            _ctx = ctx;
        }

        [RequireNeoFact]
        public void RunAScenario()
        {
            var idOfNode = _ctx.RunScenario<OneReasonablyComplexNode>().IdOfNode;
            Assert.NotEqual(0, idOfNode);
        }

        [Fact]
        public void AccessEmbeddedStatementResult()
        {
            var r = StatementResultLoader.LoadFromEmbedded("SinglePerson");
            r.ShouldNotBeNull();
            r.First().As<IRecord>()["p"].As<INode>().Properties["name"].ShouldBe("Arthur");
        }
    }
}