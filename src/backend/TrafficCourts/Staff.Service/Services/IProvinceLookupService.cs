namespace TrafficCourts.Staff.Service.Services;

public interface IProvinceLookupService
{
    Task<TrafficCourts.Domain.Models.Province?> GetByProvSeqNoCtryIdAsync(int provSeqNo, int ctryId, CancellationToken cancellationToken);
    Task<List<TrafficCourts.Domain.Models.Province>> GetListAsync(CancellationToken cancellationToken);
}
