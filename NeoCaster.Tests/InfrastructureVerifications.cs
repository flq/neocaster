using System;
using System.Diagnostics;
using NeoCaster.Tests.WithDBInfrastructure;
using Xunit;

namespace NeoCaster.Tests
{
    [Collection(nameof(WetRun))]
    public class InfrastructureVerifications
    {
        private readonly Neo4JTestingContext _ctx;

        public InfrastructureVerifications(Neo4JTestingContext ctx)
        {
            _ctx = ctx;
        }

        [Fact]
        public void RunAScenario()
        {
            var idOfNode = _ctx.RunScenario<OneReasonablyComplexNode>().IdOfNode;
            Assert.NotEqual(0, idOfNode);
        }
    }
}