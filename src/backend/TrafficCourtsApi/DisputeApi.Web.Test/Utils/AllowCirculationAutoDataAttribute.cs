using AutoFixture;
using AutoFixture.Xunit2;


namespace DisputeApi.Web.Test.Utils
{
    public class AllowCirculationAutoDataAttribute : AutoDataAttribute
    {
        public AllowCirculationAutoDataAttribute(): base(() => {
            var fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        })
        {
        }
    }
}
