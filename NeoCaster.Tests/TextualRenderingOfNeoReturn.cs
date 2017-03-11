using System.IO;
using NeoCaster.Tests.WithDBInfrastructure;
using Xunit;

namespace NeoCaster.Tests
{

    [Collection(nameof(WetRun))]
    public class TextualRenderingOfNeoReturn
    {
        private readonly Neo4JTestingContext _ctx;

        public TextualRenderingOfNeoReturn(Neo4JTestingContext ctx)
        {
            _ctx = ctx;
            _ctx.RunScenario<OneReasonablyComplexNode>();
        }

        [Fact]
        public void SeeIfWeGetOutput()
        {
            var result = _ctx.RunStatement("MATCH (p:Person) RETURN p");
            using (var fs = File.OpenWrite("/Users/flq/Desktop/test.json"))
            using(var w = new StreamWriter(fs))
            {
                result.Render(w);
            }
        }

    }
}