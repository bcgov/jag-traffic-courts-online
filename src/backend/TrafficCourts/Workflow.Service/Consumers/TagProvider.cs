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
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, ConsumeContext<CheckEmailVerificationTokenRequest> context)
    {
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }
}
