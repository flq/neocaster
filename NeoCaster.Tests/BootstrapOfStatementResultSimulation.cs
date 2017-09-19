using System.ComponentModel;
using System.Linq;
using Neo4j.Driver.V1;
using NeoCaster.Tests.DryRunInfrastructure;
using NeoCaster.Tests.WithDBInfrastructure;
using Shouldly;
using Xunit;

namespace NeoCaster.Tests
{

    [Collection(nameof(WetRun))]
    public class BootstrapOfStatementResultSimulation
    {
        private readonly Neo4JTestingContext _ctx;
        private readonly StatementResultStorage _stmntStorage;

        public BootstrapOfStatementResultSimulation(Neo4JTestingContext ctx)
        {
            _ctx = ctx;
            _stmntStorage = new StatementResultStorage();
        }

        [Fact]
        public void Comparison_single_node()
        {
            _ctx.RunScenario<TwoConnectedNodes>();
            var result = _ctx.RunStatement("MATCH (p:Person)-[:LIVES]->(a) RETURN p, a").ToList();
            result.Render(_stmntStorage.Sink);

            var zipRecordsForComparison = result.Zip(_stmntStorage.ProduceDryResult(), (real, dry) => new {real, dry});

            foreach (var pair in zipRecordsForComparison)
            {
                pair.dry.Keys.SequenceEqual(pair.real.Keys).ShouldBeTrue();
                pair.dry["p"].ShouldBeAssignableTo<INode>();
                var dryNode = pair.dry["p"].As<INode>();
                dryNode.Id.ShouldBe(pair.real["p"].As<INode>().Id);
                dryNode.Properties["name"].ShouldBe("David Backend");
                dryNode.Labels.ShouldContain("Person");
            }
        }

        [Fact]
        public void Comparison_list()
        {
            _ctx.RunScenario<CollectionOfDataPoints>();
            var result = _ctx.RunStatement("MATCH (d:DataPoint) RETURN d.seqNr as seq, d.random as random, d as nod");
            result.Render(_stmntStorage.Sink);
            var zipRecordsForComparison = result.Zip(_stmntStorage.ProduceDryResult(), (real, dry) => new {real, dry});
            foreach (var pair in zipRecordsForComparison)
            {
                pair.dry.Keys.SequenceEqual(pair.real.Keys).ShouldBeTrue();
                pair.dry["nod"].ShouldBeAssignableTo<INode>();
                var dryNode = pair.dry["nod"].As<INode>();
                dryNode.Id.ShouldBe(pair.real["nod"].As<INode>().Id);
                pair.dry["seq"].ShouldBe(pair.real["seq"]);
                pair.dry["random"].ShouldBe(pair.real["random"]);
            }
        }

    }
}