using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace DisputeApi.Web.Test.Utils
{
    public class AutoMockAutoDataAttribute : AutoDataAttribute
    {
        public AutoMockAutoDataAttribute()
          : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}
