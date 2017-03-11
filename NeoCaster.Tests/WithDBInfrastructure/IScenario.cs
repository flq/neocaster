using Neo4j.Driver.V1;

namespace NeoCaster.Tests.WithDBInfrastructure
{
    public interface IScenario
    {
        void Execute(ISession session);
    }
}