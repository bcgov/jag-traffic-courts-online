namespace TrafficCourts.Collections;

public interface IPagable
{
    int? PageNumber { get; set; }

    public int? PageSize { get; set; }
}
