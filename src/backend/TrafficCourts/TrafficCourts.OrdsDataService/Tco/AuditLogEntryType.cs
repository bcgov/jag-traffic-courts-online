using Microsoft.Extensions.Logging;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.OrdsDataService.Tco;

public class AuditLogEntryType
{
#pragma warning disable IDE1006 // Naming Styles
    public string audit_log_entry_type_cd { get; set; } = string.Empty;
    public string audit_log_entry_type_dsc { get; set; } = string.Empty;
    public string audit_log_entry_type_short_dsc { get; set; } = string.Empty;
    public string active_use_yn { get; set; } = string.Empty;
    public string comment_entry_allowed_yn { get; set; } = string.Empty;
#pragma warning restore IDE1006
}


public interface IAuditLogEntryTypeRepository
{
    Task<List<AuditLogEntryType>> GetListAsync(CancellationToken cancellationToken);
}

internal class AuditLogEntryTypeRepository : TcoOrdsRepository<AuditLogEntryTypeRepository>, IAuditLogEntryTypeRepository
{
    public AuditLogEntryTypeRepository(TcoOrdsDataServiceClient client, ILogger<AuditLogEntryTypeRepository> logger)
        : base(client, "/v2/tco_audit_log_entry_types", logger)
    {
    }
    public async Task<List<AuditLogEntryType>> GetListAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, string> parameters = new()
        {
            { "active_use_yn_eq", "Y" }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseAuditLogEntryType,
            ETagCache.OneDay,
            cancellationToken);

        return response?.Rows ?? [];
    }
}
