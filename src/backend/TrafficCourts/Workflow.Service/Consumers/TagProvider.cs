using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Sagas;

namespace TrafficCourts.Workflow.Service;

/// <summary>
/// Logging tag provider for adding properties to log messages.
/// </summary>
internal static partial class TagProvider
{
    public static void RecordTags(ITagCollector collector, SendEmailVerificationEmail message)
    {
        RecordNoticeOfDisputeIdTag(collector, message.NoticeOfDisputeGuid);
        collector.Add("TicketNumber", message.TicketNumber);
    }

    public static void RecordTags(ITagCollector collector, ConsumeContext<EmailVerificationSuccessful> context)
    {
        RecordTags<EmailVerificationSuccessful>(collector, context);
        RecordNoticeOfDisputeIdTag(collector, context.Message.NoticeOfDisputeGuid);
    }

    /// <summary>
    /// Records the common consume context properties and the Notice Of DisputeGuid.
    /// </summary>
    public static void RecordTags(ITagCollector collector, ConsumeContext<CheckEmailVerificationTokenRequest> context)
    {
        RecordTags<CheckEmailVerificationTokenRequest>(collector, context);
        RecordNoticeOfDisputeIdTag(collector, context.Message.NoticeOfDisputeGuid);
    }

    /// <summary>
    /// Records the common consume context properties and the ticket number.
    /// </summary>
    public static void RecordTags(ITagCollector collector, ConsumeContext<DisputeApproved> context)
    {
        RecordTags<DisputeApproved>(collector, context);
        Logging.TagProvider.RecordTicketNumber(collector, context.Message.TicketFileNumber);
    }

    /// <summary>
    /// Records the common consume context properties and the ticket number.
    /// </summary>
    public static void RecordTags(ITagCollector collector, ConsumeContext<SubmitNoticeOfDispute> context)
    {
        RecordTags<SubmitNoticeOfDispute>(collector, context);
        Logging.TagProvider.RecordTicketNumber(collector, context.Message.TicketNumber);
    }

    public static void RecordTags(ITagCollector collector, Exception exception)
    {
        if (exception is Arc.Dispute.Client.ApiException apiException)
        {
            collector.Add("StatusCode", apiException.StatusCode.ToString());
            collector.Add("Response", apiException.Response);
        }
    }

    public static void RecordCountNumberTag(ITagCollector collector, int count)
    {
        collector.Add("CountNumber", count);
    }

    public static void RecordTags(ITagCollector collector, VerifyEmailAddressState state)
    {
        RecordNoticeOfDisputeIdTag(collector, state.CorrelationId);

        if (!string.IsNullOrEmpty(state.TicketNumber))
        {
            collector.Add("TicketNumber", state.TicketNumber);
        }
    }

    public static void RecordDisputeIdTag(ITagCollector collector, long disputeId)
    {
        collector.Add("DisputeId", disputeId);
    }

    /// <summary>
    /// Adds common consume context properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collector"></param>
    /// <param name="context"></param>
    private static void RecordTags<T>(ITagCollector collector, ConsumeContext<T> context) where T : class
    {
        if (context.MessageId is not null) collector.Add("MessageId", context.MessageId);
        if (context.CorrelationId is not null) collector.Add("CorrelationId", context.CorrelationId);
        if (context.RequestId is not null) collector.Add("RequestId", context.RequestId);

        collector.Add("MessageType", typeof(T).Name);
        collector.Add("SentTime", context.SentTime);
    }

    private static void RecordNoticeOfDisputeIdTag(ITagCollector collector, Guid noticeOfDisputeGuid)
    {
        collector.Add("NoticeOfDisputeId", noticeOfDisputeGuid);
    }
}
