using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace DisputeApi.Web.Test.Utils
{
    public class TCOAutoDataAttribute : AutoDataAttribute
    {
        public TCOAutoDataAttribute()
          : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}
