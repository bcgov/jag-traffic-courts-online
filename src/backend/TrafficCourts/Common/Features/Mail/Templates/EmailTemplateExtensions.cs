using Microsoft.Extensions.DependencyInjection;

namespace TrafficCourts.Common.Features.Mail.Templates;

public static class EmailTemplateExtensions
{
    public static IServiceCollection AddEmailTemplates(this IServiceCollection services)
    {
        services.AddTransient<ICancelledDisputeEmailTemplate, CancelledDisputeEmailTemplate>();
        services.AddTransient<IConfirmationEmailTemplate, ConfirmationEmailTemplate>();
        services.AddTransient<IProcessingDisputeEmailTemplate, ProcessingDisputeEmailTemplate>();
        services.AddTransient<IRejectedDisputeEmailTemplate, RejectedDisputeEmailTemplate>();

        return services;
    }
}
