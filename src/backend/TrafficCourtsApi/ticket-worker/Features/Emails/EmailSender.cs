using FluentEmail.Core;
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

        void SendUsingTemplate(string to, string subject, TicketDisputeContract model);

    }
    public enum EmailTemplate
    {
        EmailConfirmation,
        ChangeEmail
    }

    public class EmailSender : IEmailSender
    {

        private readonly IFluentEmail _email;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IFluentEmail email, ILogger<EmailSender> logger)
        {
            _email = email;
            _logger = logger;
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
            String dataScheme = String.Format("data:image/{0};base64,{1}==",
                                mimeType,base64Data);
            return dataScheme;
            
        }

        public async void SendUsingTemplate(string to, string subject, TicketDisputeContract model)
        {
            DisputeEmail emailModel = constructDisputeEmailModel(model);
            emailModel.LogoImage = dataURIScheme("png", "ticket-worker.Features.Emails.Resources.bc-gov-logo.png");
            var result = await _email.To(to)
            .Subject(subject)
            .UsingTemplateFromEmbedded("ticket-worker.Features.Emails.Resources.submissiontemplate.liquid", emailModel, this.GetType().GetTypeInfo().Assembly)
            .SendAsync();

            if (!result.Successful)
            {
                _logger.LogError("Failed to send an email");
            }
        }

        private DisputeEmail constructDisputeEmailModel(TicketDisputeContract model)
        {
            DisputeEmail disputeEmailModel = new DisputeEmail();
                
            disputeEmailModel.ConfirmationNumber = model.ConfirmationNumber;
            disputeEmailModel.ViolationDate = model.ViolationDate;
            disputeEmailModel.ViolationTicketNumber = model.ViolationTicketNumber;
            disputeEmailModel.ViolationTime = model.ViolationTime;
            disputeEmailModel.CountOneDescription = model.Offences[0].OffenceDescription;
            disputeEmailModel.CountTwoDescription = model.Offences[1].OffenceDescription;
            disputeEmailModel.CountThreeDescription = model.Offences[2].OffenceDescription;
            switch (model.Offences[0].OffenceAgreementStatus)
            {
                case "PAY":
                    disputeEmailModel.CountOneAction = "Pay for this count ";
                    break;
                case "DISPUTE":
                    disputeEmailModel.CountOneAction = "Dispute the charge";
                    break;
                case "REDUCTION":
                    disputeEmailModel.CountOneAction = "Request a fine reduction and/or more time to pay";
                    break;
                default:
                    disputeEmailModel.CountOneAction = "Do no action at this time";
                    break;
            }
            switch (model.Offences[1].OffenceAgreementStatus)
            {
                case "PAY":
                    disputeEmailModel.CountTwoAction = "Pay for this count ";
                    break;
                case "DISPUTE":
                    disputeEmailModel.CountTwoAction = "Dispute the charge";
                    break;
                case "REDUCTION":
                    disputeEmailModel.CountTwoAction = "Request a fine reduction and/or more time to pay";
                    break;
                default:
                    disputeEmailModel.CountTwoAction = "Do no action at this time";
                    break;
            }
            switch (model.Offences[2].OffenceAgreementStatus)
            {
                case "PAY":
                    disputeEmailModel.CountThreeAction = "Pay for this count ";
                    break;
                case "DISPUTE":
                    disputeEmailModel.CountThreeAction = "Dispute the charge";
                    break;
                case "REDUCTION":
                    disputeEmailModel.CountThreeAction = "Request a fine reduction and/or more time to pay";
                    break;
                default:
                    disputeEmailModel.CountThreeAction = "Do no action at this time";
                    break;
            }
            disputeEmailModel.CountOneAmount = model.Offences[0].AmountDue;
            disputeEmailModel.CountTwoAmount = model.Offences[1].AmountDue;
            disputeEmailModel.CountThreeAmount = model.Offences[2].AmountDue;
            disputeEmailModel.CountOneWillAppear = model.Offences[0].ReductionAppearInCourt == true? "Yes" : "No";
            disputeEmailModel.CountTwoWillAppear = model.Offences[1].ReductionAppearInCourt == true ? "Yes" : "No";
            disputeEmailModel.CountThreeWillAppear = model.Offences[2].ReductionAppearInCourt == true ? "Yes" : "No";
            disputeEmailModel.RequireInterpreter = model.Additional.InterpreterRequired ? "I require an interpreter" : "I do not require an interpreter";
            disputeEmailModel.InterpreterLanguage = model.Additional.InterpreterLanguage;
            disputeEmailModel.NumberofWitnesses = model.Additional.NumberOfWitnesses == null ? 0 : (int)model.Additional.NumberOfWitnesses;
            disputeEmailModel.CallWitness = model.Additional.WitnessPresent ? "I intend to call witness(es)" : "I do not intend to call witness(es)";
            

            return disputeEmailModel;
        }
    }

    
}
