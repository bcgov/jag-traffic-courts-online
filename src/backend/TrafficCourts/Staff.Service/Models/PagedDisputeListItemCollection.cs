using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using X.PagedList;

namespace TrafficCourts.Staff.Service.Models
{
    public class PagedDisputeListItemCollection : TrafficCourts.Domain.Models.PagedCollection<DisputeListItem>
    {
        public PagedDisputeListItemCollection(IPagedList<DisputeListItem> values) : base(values)
        {
        }
    }
}
