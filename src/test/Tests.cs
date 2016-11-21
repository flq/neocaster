using System.Linq;
using Xunit;

namespace NeoCaster.Tests
{
    [Collection("DB")]
    public class IsThingsWorking
    {
        private Neo4jTestingContext _ctx;
        public IsThingsWorking(Neo4jTestingContext ctx)
        {
            _ctx = ctx;
        }

        [Fact]
        public void ContextIsInjected() 
        {
            Assert.NotNull(_ctx);
        }

        [Fact]
        public void CanUseSession() 
        {
            _ctx.WithSession(s => {
                var records = s.Run("MATCH (n) RETURN count(n) as count").ToList();
                Assert.Equal(1, records.Count);
            });
        }
    }
}
