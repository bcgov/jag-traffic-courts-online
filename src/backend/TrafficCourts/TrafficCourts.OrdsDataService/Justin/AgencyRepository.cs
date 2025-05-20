using Microsoft.Extensions.Logging;
using TrafficCourts.OrdsDataService.Tco;

namespace TrafficCourts.OrdsDataService.Justin;

internal static class AgencyType
{
    public const string ResourceOnlyAgency = "RES";
    public const string EnforcementAgency = "PO";
    public const string CrownCounsel = "CO";
    public const string CourtRegistry = "CTH";
    public const string ReferralAgency = "REF";
    public const string VictimServicesAgency = "VIC";
    public const string GovernmentAgency = "GOV";
    public const string AssessmentCentre = "ASC";
    public const string Institution = "INS";
    public const string OrderRegistry = "OR";
    public const string Bailiff = "BLF";
    public const string OffSiteStorageFacility = "OSF";
    public const string SheriffAgency = "SHF";
    public const string PrivateProsecutor = "PP";
    public const string ExternalDataAccess = "EDA";
}

internal class AgencyRepository : OrdsRepository<AgencyRepository>, IAgencyRepository
{
    public AgencyRepository(TcoOrdsDataServiceClient ordsClient, ILogger<AgencyRepository> logger)
        : base(ordsClient, "/v2/justin_agencies", logger)
    {
    }

    public async Task<List<Agency>> GetListAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, string> parameters = new()
        {
            { "agen_active_yn_eq", "Y" }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseAgency,
            ETagCache.OneDay,
            cancellationToken);

        return response.Rows ?? [];
    }

    public async Task<List<Agency>> GetListAsync(string agencyType, CancellationToken cancellationToken)
    {
        Dictionary<string, string> parameters = new()
        {
            { "cdat_agency_type_cd_eq", agencyType },
            { "agen_active_yn_eq", "Y" }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseAgency,
            ETagCache.OneDay,
            cancellationToken);

        return response.Rows ?? [];
    }

    public async Task<Agency?> GetAsync(decimal agen_id, CancellationToken cancellationToken)
    {
        Dictionary<string, string> parameters = new()
        {
            { "agen_id_eq", agen_id.ToString() }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseAgency,
            ETagCache.NoCache, // do not cache small responses
            cancellationToken);

        var agency = response?.Rows?.SingleOrDefault();
        return agency;
    }
}
