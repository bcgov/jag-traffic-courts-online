using MassTransit;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service;

/// <summary>
/// Logging tag provider for adding properties to log messages.
/// </summary>
internal static partial class TagProvider
{
    public static void RecordTags(ITagCollector collector, ConsumeContext<EmailVerificationSuccessful> context)
    {
        RecordTags<EmailVerificationSuccessful>(collector, context);
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, ConsumeContext<CheckEmailVerificationTokenRequest> context)
    {
        RecordTags<CheckEmailVerificationTokenRequest>(collector, context);
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, ConsumeContext<DisputeApproved> context)
    {
        RecordTags<DisputeApproved>(collector, context);
        collector.Add("TicketNumber", context.Message.TicketFileNumber);
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
}
