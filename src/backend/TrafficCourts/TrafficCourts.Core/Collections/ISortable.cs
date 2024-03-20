namespace TrafficCourts.Collections;

public interface ISortable
{
    public List<string>? SortBy { get; set; }

    public List<SortDirection>? SortDirection { get; set; }
}
