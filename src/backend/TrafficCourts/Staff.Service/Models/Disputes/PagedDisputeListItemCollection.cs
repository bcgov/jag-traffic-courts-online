using TrafficCourts.Domain.Models;
using X.PagedList;

namespace TrafficCourts.Staff.Service.Models
{
    public class PagedDisputeListItemCollection : PagedCollection<DisputeListItem>
    {
        public PagedDisputeListItemCollection(IPagedList<DisputeListItem> values) : base(values)
        {
        }
    }
}
