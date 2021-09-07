using FluentEmail.Core;
using Gov.TicketWorker.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;

namespace Gov.TicketWorker.Features.Emails
{

    public interface IEmailSender
    {

        Task<bool> SendUsingTemplate(string to, string subject, TicketDisputeContract model);

    }
    public enum EmailTemplate
    {
        EmailConfirmation,
        ChangeEmail
    }

    class EmailSender : IEmailSender
    {

        private readonly IFluentEmail _email;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IFluentEmail email, ILogger<EmailSender> logger)
        {
            _email = email;
            _logger = logger;
        }

        public async Task<bool> SendUsingTemplate(string to, string subject, TicketDisputeContract model)
        {
            DisputeEmail emailModel = constructDisputeEmailModel(model);
                var result = await _email.To(to)
                .Subject(subject)
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/EmailResources/_layout.liquid", emailModel)
                .SendAsync();

                if (!result.Successful)
                {
                    _logger.LogError("Failed to send an email");
                }
            
            return result.Successful;
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
            disputeEmailModel.LogoPath = $"{Directory.GetCurrentDirectory()}/EmailResources/bc-gov-logo.png";
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
            disputeEmailModel.CountOneWillAppear = (bool)model.Offences[0].ReductionAppearInCourt ? "Yes" : "No";
            disputeEmailModel.CountTwoWillAppear = (bool)model.Offences[1].ReductionAppearInCourt ? "Yes" : "No";
            disputeEmailModel.CountThreeWillAppear = (bool)model.Offences[2].ReductionAppearInCourt ? "Yes" : "No";
            disputeEmailModel.RequireInterpreter = model.Additional.InterpreterRequired ? "I require an interpreter" : "I do not require an interpreter";
            disputeEmailModel.InterpreterLanguage = model.Additional.InterpreterLanguage;
            disputeEmailModel.NumberofWitnesses = (int)model.Additional.NumberOfWitnesses;
            disputeEmailModel.CallWitness = model.Additional.WitnessPresent ? "I intend to call witness(es)" : "I do not intend to call witness(es)";

            return disputeEmailModel;
        }
    }

    
}
