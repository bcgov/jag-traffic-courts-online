using TrafficCourts.Domain.Models;

namespace TrafficCourts.Staff.Service.Features.Occam.Disputes
{
    public class PagedOccamDisputeListItemCollection
    {
        public PagedOccamDisputeListItemCollection(IEnumerable<OccamDisputeListItemModel> items, int pageNumber, int pageSize, int totalRows)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRows = totalRows;
            TotalPages = (int)Math.Ceiling((double)totalRows / pageSize);
        }

        public IEnumerable<OccamDisputeListItemModel> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
    }
}
