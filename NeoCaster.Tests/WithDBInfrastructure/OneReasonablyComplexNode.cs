using System;
using System.Diagnostics;
using System.Linq;
using Neo4j.Driver.V1;

namespace NeoCaster.Tests.WithDBInfrastructure
{
    public class OneReasonablyComplexNode : IScenario
    {
        public void Execute(ISession session)
        {
            session.Run("CREATE (:Foo)"); //To avoid actually getting the id 0.
            var result = session.Run(
                "CREATE (p:Person { firstName: 'Arthur', lastName: 'Brannigan', isMarried: true, dob: 946760762}) RETURN id(p) AS id");
            var stuff = result.ToList();
            IdOfNode = stuff.First()["id"].As<int>();
        }

        public int IdOfNode { get; private set; }
    }
}