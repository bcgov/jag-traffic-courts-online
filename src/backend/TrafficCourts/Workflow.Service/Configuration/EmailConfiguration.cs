namespace TrafficCourts.Workflow.Service.Configuration
{
    /// <summary>
    /// Represents the email sending configuration
    /// </summary>
    public class EmailConfiguration
    {
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
        /// The allowed email domains we can send to. Each entry will have format '@example.com'.
        /// </summary>
        public IList<string> Allowed
        {
            get
            {
                if (_allowed is null)
                {
                    string allowList = AllowList;
                    if (string.IsNullOrEmpty(allowList))
                    {
                        _allowed = Array.Empty<string>();
                    }
                    else
                    {
                        _allowed = allowList.Split(",")
                            .Select(_ => _.Trim())
                            .ToList();
                    }
                }

                return _allowed;
            }
        }
    }
}
