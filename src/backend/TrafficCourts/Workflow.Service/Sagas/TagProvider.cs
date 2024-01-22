using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Sagas;

namespace TrafficCourts.Workflow.Service;

/// <summary>
/// Logging tag provider for adding properties to log messages.
/// </summary>
internal static partial class TagProvider
{
    public static void RecordTags(ITagCollector collector, BehaviorContext<VerifyEmailAddressState, SubmitNoticeOfDispute> context)
    {
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, BehaviorContext<VerifyEmailAddressState, RequestEmailVerification> context)
    {
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, BehaviorContext<VerifyEmailAddressState, CheckEmailVerificationTokenRequest> context)
    {
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, BehaviorContext<VerifyEmailAddressState, EmailVerificationSuccessful> context)
    {
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, BehaviorContext<VerifyEmailAddressState, ResendEmailVerificationEmail> context)
    {
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, BehaviorContext<VerifyEmailAddressState, NoticeOfDisputeSubmitted> context)
    {
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
    }

    public static void RecordTags(ITagCollector collector, BehaviorContext<VerifyEmailAddressState, SendEmailVerificationFailed> context)
    {
        collector.Add(nameof(context.Message.NoticeOfDisputeGuid), context.Message.NoticeOfDisputeGuid);
        collector.Add(nameof(context.Message.Reason), context.Message.Reason);
    }
}
