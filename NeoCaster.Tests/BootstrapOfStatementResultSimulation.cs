using System;
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver.V1;
using NeoCaster.Tests.DryRunInfrastructure;
using NeoCaster.Tests.WithDBInfrastructure;
using Shouldly;
using Xunit;

namespace NeoCaster.Tests
{

    [Collection(nameof(IntegrationRun))]
    public class BootstrapOfStatementResultSimulation
    {
        private readonly Neo4JIntegrationTesting _ctx;
        private readonly StatementResultStorage _stmntStorage;

        public BootstrapOfStatementResultSimulation(Neo4JIntegrationTesting ctx)
        {
            _ctx = ctx;
            _stmntStorage = new StatementResultStorage();
        }

        [RequireNeoFact]
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

        [RequireNeoFact]
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

        [RequireNeoFact]
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

        [RequireNeoFact]
        public void assert_multirow_result_produces_additional_equal_nodes()
        {
            var multiRow = "MATCH (p:Person)-[r:OWNS]->(n) WHERE id(p) = {id} RETURN p, r, n";
            var id = _ctx.RunScenario<OneToNNodesAndRelationships>().IdOfPerson;
            var result = _ctx.RunStatement(multiRow, new { id }).ToList();

            // not reusing reference + raw output in neo browser suggests
            // that the node multiple times.
            result[0]["p"].ShouldNotBeSameAs(result[1]["p"]); 
            result[0]["p"].ShouldBe(result[1]["p"]); //Nodes implement equality based on id

        }

        [RequireNeoFact]
        public void Compare_asymetric_resultset_simple()
        {
            const string singleRow = @"MATCH (p:Person)-[r:OWNS]->(n) 
            WITH p, collect(n) as products
            WHERE id(p) = {id} RETURN p, products";

            var id = _ctx.RunScenario<OneToNNodesAndRelationships>().IdOfPerson;
            var result = _ctx.RunStatement(singleRow, new { id }).ToList();
            result.Render(_stmntStorage.Sink);
            var zip = result.Zip(_stmntStorage.ProduceDryResult(), (real, dry) => (real, dry)).ToList();
            var (r, d) = zip[0];
            var realPerson = r.Values["p"].As<INode>();
            var dryPerson = d.Values["p"].As<INode>();
            dryPerson.Labels[0].ShouldBe(realPerson.Labels[0]);
            dryPerson.Properties["name"].ShouldBe(realPerson.Properties["name"]);

            var realProducts = r.Values["products"].As<IList<object>>().OfType<INode>().ToList();
            var dryProducts = d.Values["products"].As<IList<object>>().OfType<INode>().ToList();

            dryProducts.Count.ShouldBe(2);
            dryProducts[0].Labels[0].ShouldBe(realProducts[0].Labels[0]);
            dryProducts[0].Properties["name"].ShouldBe(realProducts[0].Properties["name"]);
            dryProducts[1].Properties["name"].ShouldBe(realProducts[1].Properties["name"]);
        }


    }
}