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
            var person = result[0];
            person.Name.ShouldBe("Arthur");
            person.Dob.ShouldBe(12312333);
            person.DoesNotExistOnRecord.ShouldBeNull();
            person.DoesExistOnRecord.ShouldBe(true);
        }

        [Fact]
        public void Query_list_of_persons()
        {
            void AssertName(Person p, string name) => p.Name.ShouldBe(name);

            var s = new DryRunSession(_ => StatementResultLoader.LoadFromEmbedded("ListOfPersons"));
            var result = s.Query<Person>("CYPHER QUERY RETURN many people").ToList();
            result.Count.ShouldBe(3);
            AssertName(result[0], "Arthur");
            AssertName(result[1], "Parsifal");
            AssertName(result[2], "Merlin");
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