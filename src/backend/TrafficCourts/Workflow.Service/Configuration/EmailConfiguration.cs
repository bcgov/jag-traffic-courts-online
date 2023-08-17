using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Workflow.Service.Configuration;

/// <summary>
/// Represents the email sending configuration
/// </summary>
public class EmailConfiguration : IValidatable
{
    public const string Section = "EmailConfiguration";

    private IList<string>? _allowed;

    /// <summary>
    /// The default valid email address to send from if no from address specified in message.
    /// </summary>
    public string? Sender { get; set; }

    /// <summary>
    /// The optoinal comma separated list of email domains that messages can sent to. Values must have the format '@example.com,@example.org'.
    /// </summary>
    public string AllowList { get; set; } = string.Empty;

    /// <summary>
    /// The URL to which a Disputant can verify their email address. ie, https://citizen-portal/email/verify.
    /// </summary>
    public string? EmailVerificationUrl { get; set; }

    /// <summary>
    /// The allowed email domains we can send to. Each entry will have format '@example.com'.
    /// </summary>
    public IList<string> Allowed
    {
        get
        {
            if (_allowed is null)
            {
                _allowed = SplitAllowList();
            }

            return _allowed;
        }
    }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Sender)) throw new SettingsValidationException(Section, nameof(Sender), "is required");

        if (!string.IsNullOrEmpty(AllowList))
        {
            var allowed = SplitAllowList();
            if (allowed.Any(_ => !_.StartsWith("@"))) throw new SettingsValidationException(Section, nameof(AllowList), "values must start with @");
        }

        if (string.IsNullOrEmpty(EmailVerificationUrl)) throw new SettingsValidationException(Section, nameof(EmailVerificationUrl), "is required");
    }


    private IList<string> SplitAllowList()
    {
        if (string.IsNullOrEmpty(AllowList))
        {
            return Array.Empty<string>();
        }

        return AllowList.Split(",")
            .Select(_ => _.Trim())
            .ToList();
    }
}
