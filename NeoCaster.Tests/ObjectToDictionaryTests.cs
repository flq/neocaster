using Shouldly;
using Xunit;

namespace NeoCaster.Tests
{
    public class ObjectToDictionaryTests
    {

        [Fact, Trait("Category", "Unit")]
        public void CanConvertAnonymousObjectToDictionary()
        {
            var o = new {FirstName = "Joe", Dob = 123456L, IsSingle = true};
            var d = o.Convert();
            d.Keys.Count.ShouldBe(3);

            d.ContainsKey("FirstName").ShouldBeTrue();
            d["FirstName"].ShouldBe(o.FirstName);

            d.ContainsKey("Dob").ShouldBeTrue();
            d["Dob"].ShouldBe(o.Dob);

            d.ContainsKey("IsSingle").ShouldBeTrue();
            d["IsSingle"].ShouldBe(o.IsSingle);
        }

    }
}