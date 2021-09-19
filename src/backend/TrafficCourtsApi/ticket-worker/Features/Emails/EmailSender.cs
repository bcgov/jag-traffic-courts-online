using FluentEmail.Core;
using FluentEmail.Core.Models;
using Gov.TicketWorker.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        /// <exception cref="SendEmailException">When sending the email failed. The inner exception will have the cause.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="to"/>, <paramref name="subject"/> or <paramref name="model"/> is null.
        /// </exception>
        Task SendUsingTemplate(string to, string subject, TicketDisputeContract model);

    }
    public enum EmailTemplate
    {
        EmailConfirmation,
        ChangeEmail
    }

    [Serializable]
    public class SendEmailException : Exception
    {
        public SendEmailException(string message, Exception inner) : base(message, inner) { }
        protected SendEmailException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public class EmailSender : IEmailSender
    {

        private readonly IFluentEmail _email;
        private readonly ILogger<EmailSender> _logger;

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

        public async Task SendUsingTemplate(string to, string subject, TicketDisputeContract model)
        {
            if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException(nameof(to));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException(nameof(subject));
            if (model == null) throw new ArgumentNullException(nameof(model));

            try
            {
                DisputeEmail emailModel = new DisputeEmail(model);

                emailModel.LogoImage = dataURIScheme("png", "ticket-worker.Features.Emails.Resources.bc-gov-logo.png");

                var result = await _email
                    .To(to)
                    .Subject(subject)
                    .UsingTemplateFromEmbedded("ticket-worker.Features.Emails.Resources.submissiontemplate.liquid", emailModel, this.GetType().GetTypeInfo().Assembly)
                    .SendAsync();

                
                if (!result.Successful)
                {
                    _logger.LogError("Failed to send an email");
                }
            }
            catch (Exception e)
            {
                throw new SendEmailException("Failed to send email", e);
            }
        }
    }
}
