using System.Collections.Generic;
using System.Linq;
using NeoCaster.Tests.DryRunInfrastructure;
using Shouldly;
using Xunit;

namespace NeoCaster.Tests.DryRunTests
{
    public class QueryTests
    {
        [Theory, ClassData(typeof(QueryTestCases))]
        public void Query_SingleNode_AsNode_Or_Properties(string embeddedReturn, string cypherQuery)
        {
            var s = new DryRunSession(_ => StatementResultLoader.LoadFromEmbedded(embeddedReturn));
            var result = s.Query<Person>(cypherQuery).ToList();
            result.Count.ShouldBe(1);
            result[0].Name.ShouldBe("Arthur");
            result[0].Dob.ShouldBe(12312333);
            result[0].DoesNotExistOnRecord.ShouldBeNull();

        }

        public class QueryTestCases : AbstractClassData
        {
            protected override IEnumerator<object[]> TestData()
            {
                yield return Data("SinglePersonAsRow", "CYPHER QUERY RETURN prop1, prop2");
                yield return Data("SinglePerson", "CYPHER QUERY RETURN node");
            }
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public long Dob { get; set; }
        public bool? DoesNotExistOnRecord { get; set; }
        public bool DoesExistOnRecord { get; set; }
    }
}