using System;
using System.Collections.Generic;
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

    public class TwoConnectedNodes : IScenario
    {
        public void Execute(ISession session)
        {
            var result = session.Run(
                "CREATE (p:Person { name: 'David Backend' }), (a:City { name: 'Mulhouse' }), (p)-[:LIVES { since: 1245145 }]->(a) RETURN id(p) AS id");
            var stuff = result.ToList();
            IdOfNode = stuff.First()["id"].As<int>();
        }

        public int IdOfNode { get; private set; }
    }

    public class CollectionOfDataPoints : IScenario
    {
        public void Execute(ISession session)
        {
            const string statement = "CREATE (:DataPoint { seqNr: {seq}, random: {rand} })";
            var parameters = new Dictionary<string, object>();

            // Tried to use a transaction, didn't work
            for (int i = 0; i < 10; i++)
            {
                parameters["seq"] = i;
                parameters["rand"] = Guid.NewGuid().ToString();
                session.Run(statement, parameters);
            }
        }
    }
}