using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

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
