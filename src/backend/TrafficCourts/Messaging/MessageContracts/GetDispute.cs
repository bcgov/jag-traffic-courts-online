using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Messaging.MessageContracts;

public class GetDisputeRequest
{
    public Guid NoticeOfDisputeGuid { get; set; }
}