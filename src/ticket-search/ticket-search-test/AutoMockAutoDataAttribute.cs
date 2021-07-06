using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Gov.TicketSearch.Test
{
    public class AutoMockAutoDataAttribute : AutoDataAttribute
    {
        public AutoMockAutoDataAttribute()
          : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}
