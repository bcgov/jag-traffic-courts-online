using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
namespace TrafficCourts.Workflow.Service.Mappers;

public class Mapper
{
    public static EmailHistory ToEmailHistory(DisputantEmailSent src)
    {
        ArgumentNullException.ThrowIfNull(src);

        EmailHistory target = new EmailHistory();

        target.EmailSentTs = src.SentAt;
        target.ToEmailAddress = src.Message?.To is not null ? src.Message.To : "unknown";
        target.SuccessfullySent = EmailHistorySuccessfullySent.Y;
        target.Subject = src.Message?.Subject is not null ? src.Message.Subject : "unknown";
        target.HtmlContent = src.Message?.HtmlContent is not null ? src.Message.HtmlContent : "";
        target.PlainTextContent = src.Message?.TextContent is not null ? src.Message.TextContent : "";
        target.OccamDisputeId = src.OccamDisputeId;
        target.FromEmailAddress = src.Message?.From is not null ? src.Message.From : "unknown";
        
        return target;
    }

    public static EmailHistory ToEmailHistory(DisputantEmailFiltered src)
    {
        ArgumentNullException.ThrowIfNull(src);

        EmailHistory target = new EmailHistory();

        target.EmailSentTs = src.FilteredAt;
        target.ToEmailAddress = src.Message?.To is not null ? src.Message.To : "unknown";
        target.SuccessfullySent = EmailHistorySuccessfullySent.N;
        target.Subject = src.Message?.Subject is not null ? src.Message.Subject : "unknown";
        target.HtmlContent = src.Message?.HtmlContent is not null ? src.Message.HtmlContent : "";
        target.PlainTextContent = src.Message?.TextContent is not null ? src.Message.TextContent : "";
        target.OccamDisputeId = src.OccamDisputeId;
        target.FromEmailAddress = src.Message?.From is not null ? src.Message.From : "unknown";

        return target;
    }
}
