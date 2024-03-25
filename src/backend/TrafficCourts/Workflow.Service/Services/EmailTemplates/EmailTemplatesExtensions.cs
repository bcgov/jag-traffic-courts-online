namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public static class EmailTemplatesExtensions
{
    /// <summary>
    /// Adds the email templates defined in the workflow service.
    /// </summary>
    /// <param name="services"></param>
    public static void AddEmailTemplates(this IServiceCollection services)
    {
        services.AddTransient<ICancelledDisputeEmailTemplate, CancelledDisputeEmailTemplate>();
        services.AddTransient<IDisputantEmailUpdateSuccessfulTemplate, DisputantEmailUpdateSuccessfulTemplate>();
        services.AddTransient<IDisputeSubmittedEmailTemplate, DisputeSubmittedEmailTemplate>();
        services.AddTransient<IDisputeUpdateRequestAcceptedTemplate, DisputeUpdateRequestAcceptedTemplate>();
        services.AddTransient<IDisputeUpdateRequestReceivedTemplate, DisputeUpdateRequestReceivedTemplate>();
        services.AddTransient<IDisputeUpdateRequestRejectedTemplate, DisputeUpdateRequestRejectedTemplate>();
        services.AddTransient<IProcessingDisputeEmailTemplate, ProcessingDisputeEmailTemplate>();
        services.AddTransient<IRejectedDisputeEmailTemplate, RejectedDisputeEmailTemplate>();
        services.AddTransient<IVerificationEmailTemplate, VerificationEmailTemplate>();
    }
}
