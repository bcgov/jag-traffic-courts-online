using System.Text.Json.Serialization;

namespace TrafficCourts.OrdsDataService;

[JsonSerializable(typeof(Justin.Agency))]
[JsonSerializable(typeof(Justin.City))]
[JsonSerializable(typeof(Justin.Country))]
[JsonSerializable(typeof(Justin.Language))]
[JsonSerializable(typeof(Justin.Province))]
[JsonSerializable(typeof(Justin.Statute))]
[JsonSerializable(typeof(Tco.AuditLogEntryType))]
[JsonSerializable(typeof(Tco.OrdsDisputeCaseFileSummary))]
[JsonSerializable(typeof(Tco.DisputeStatusType))]
[JsonSerializable(typeof(Occam.OccamDispute))]
[JsonSerializable(typeof(OrdsDataServiceCollectionResponse<Justin.Agency>))]
[JsonSerializable(typeof(OrdsDataServiceCollectionResponse<Justin.City>))]
[JsonSerializable(typeof(OrdsDataServiceCollectionResponse<Justin.Country>))]
[JsonSerializable(typeof(OrdsDataServiceCollectionResponse<Justin.Language>))]
[JsonSerializable(typeof(OrdsDataServiceCollectionResponse<Justin.Province>))]
[JsonSerializable(typeof(OrdsDataServiceCollectionResponse<Justin.Statute>))]
[JsonSerializable(typeof(OrdsDataServiceCollectionResponse<Tco.AuditLogEntryType>))]
[JsonSerializable(typeof(OrdsDataServiceCollectionResponse<Tco.DisputeStatusType>))]
[JsonSerializable(typeof(OrdsDataServicePagedCollectionResponse<Tco.OrdsDisputeCaseFileSummary>))]
[JsonSerializable(typeof(OrdsDataServicePagedCollectionResponse<Occam.OccamDispute>))]
internal partial class JsonContext : JsonSerializerContext
{
}
