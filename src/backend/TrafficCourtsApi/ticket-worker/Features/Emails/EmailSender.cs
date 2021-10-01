using FluentEmail.Core;
using FluentEmail.Core.Models;
using Gov.TicketWorker.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;

namespace Gov.TicketWorker.Features.Emails
{

    public interface IEmailSender
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="model"></param>
        /// <exception cref="SendEmailException">When sending the email failed. The inner exception will have the cause. the exception message will be logged.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="to"/>, <paramref name="subject"/> or <paramref name="model"/> is null.
        /// </exception>
        Task SendUsingTemplateAsync(string to, string subject, TicketDisputeContract model);

    }
    public enum EmailTemplate
    {
        EmailConfirmation,
        ChangeEmail
    }

    [Serializable]
    public class SendEmailException : Exception
    {
        public List<string> Messages { get; } = new List<string>();
        public SendEmailException(string message, Exception inner) : base(message, inner) 
        {
            Messages.Add(message);
        }

        public SendEmailException(IList<string> messages) : base(messages.Count>0?messages[0]:"No message supplied")
        {
            Messages.AddRange(messages);
        }

        protected SendEmailException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class EmailSender : IEmailSender
    {

        private readonly IFluentEmail _email;
        private readonly ILogger<EmailSender> _logger;
        private readonly IEmailFilter _emailFilter;
        private AsyncPolicyWrap<SendResponse> _policyWrap;

        public EmailSender(IFluentEmail email, ILogger<EmailSender> logger, IEmailFilter emailFilter)
        {
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailFilter = emailFilter ?? throw new ArgumentNullException(nameof(emailFilter));

            AsyncRetryPolicy<SendResponse> retryPolicy = Policy.HandleResult<SendResponse>(r=> !r.Successful)
                .Or<Exception>(e=> (e is SmtpFailedRecipientException )||(e is SmtpFailedRecipientsException))
                .WaitAndRetryAsync(
                    2, 
                    retryAttempt => {
                        var timeToWait = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                        _logger.LogInformation($"Waiting {timeToWait.TotalSeconds} seconds to retry sending email");
                        return timeToWait;
                    }
                    );
            AsyncCircuitBreakerPolicy<SendResponse> circuitBreakerPolicy = Policy.HandleResult<SendResponse>(r => !r.Successful)
                .CircuitBreakerAsync(20, TimeSpan.FromSeconds(10));

            _policyWrap = Policy.WrapAsync<SendResponse>(retryPolicy, circuitBreakerPolicy);
        }

        public EmailSender(IFluentEmail email, ILogger<EmailSender> logger)
        {
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string dataURIScheme(string mimeType, string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resource);
            byte[] bytes;
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();

            string base64Data = Convert.ToBase64String(bytes);
            string dataScheme = $"data:image/{mimeType};base64,{base64Data}==";
            return dataScheme;
            
        }

        public async Task SendUsingTemplateAsync(string to, string subject, TicketDisputeContract model)
        {
            if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException(nameof(to));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException(nameof(subject));
            if (model == null) throw new ArgumentNullException(nameof(model));

            try
            {
                _logger.LogDebug("prepare to send email to {to}", to);
                DisputeEmail emailModel = new DisputeEmail(model);

                emailModel.LogoImage = dataURIScheme("png", "ticket-worker.Features.Emails.Resources.bc-gov-logo.png");

                var email = _email
                    .To(to)
                    .Subject(subject)
                    .UsingTemplateFromEmbedded("ticket-worker.Features.Emails.Resources.submissiontemplate.liquid", emailModel, this.GetType().GetTypeInfo().Assembly, true);

                if (_emailFilter.IsAllowed(to))
                {
                    //var result = await _policyWrap.ExecuteAsync(() => email.SendAsync());
                    //comment out policyWrap for testing sending email. After sending email problem resolved, we can add
                    //retry policy.
                    //issue: If there are multiple NotificationRequest in the queue, when ticket worker consume the queue
                    //email sending always through exception like "Invlid user state".
                    //need research on this.
                    //If sending email one by one, it works fine.
                    var result = await email.SendAsync();

                    if (!result.Successful)
                    {
                        _logger.LogWarning("sending email is not successful {ErrorMessages}", result.ErrorMessages);
                        throw new SendEmailException(result.ErrorMessages);                        
                    }
                    else
                    {
                        _logger.LogInformation("Email sent successfully");
                    }
                }
                else
                {
                    _logger.LogInformation("The target email address is not allowed to be sent to.");
                }
               
            }
            catch (Exception e)
            {
                _logger.LogError(e, "send email failed");
                throw new SendEmailException("Failed to send email", e);
            }
        }
    }
}
