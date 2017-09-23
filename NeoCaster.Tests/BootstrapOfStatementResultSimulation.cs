using System;
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
        public void Comparison_two_nodes()
        {
            _ctx.RunScenario<TwoConnectedNodes>();
            var result = _ctx.RunStatement("MATCH (p:Person)-[:LIVES]->(a) RETURN p, a").ToList();
            result.Render(_stmntStorage.Sink);
            var zipRecordsForComparison = result.Zip(_stmntStorage.ProduceDryResult(), (real, dry) => (real, dry));

            void AssertValues(string key, ValueTuple<IRecord,IRecord> nodes, string label)
            {
                var (real, dry) = nodes;
                dry.Keys.SequenceEqual(real.Keys).ShouldBeTrue();
                var dryNode = dry[key].As<INode>();
                var realNode = real[key].As<INode>();
                dryNode.Id.ShouldBe(realNode.Id);
                dryNode.Properties["name"].ShouldBe(realNode.Properties["name"]);
                dryNode.Labels.ShouldContain(label);
            }

            foreach (var pair in zipRecordsForComparison)
            {
                AssertValues("p", pair, "Person");
                AssertValues("a", pair, "City");

            }
        }

        [Fact]
        public void Two_nodes_with_relationship()
        {
            _ctx.RunScenario<TwoConnectedNodes>();
            var result = _ctx.RunStatement("MATCH (p:Person)-[l:LIVES]->(a) RETURN p, l, a").ToList();
            result.Render(_stmntStorage.Sink);
            var zipRecordsForComparison = result.Zip(_stmntStorage.ProduceDryResult(), (real, dry) => (real, dry));
            var (r, d) = zipRecordsForComparison.First();
            var realRel = r["l"].As<IRelationship>();
            var dryRel = d["l"].As<IRelationship>();
            dryRel.StartNodeId.ShouldBe(realRel.StartNodeId);
            dryRel.EndNodeId.ShouldBe(realRel.EndNodeId);
            dryRel.Type.ShouldBe(realRel.Type);
            dryRel["since"].ShouldBe(realRel["since"]);
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