using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;

namespace Gov.TicketWorker.Models
{
    public class DisputeEmail
    {

        public DisputeEmail(TicketDisputeContract model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            this.ConfirmationNumber = model.ConfirmationNumber;
            this.ViolationDate = model.ViolationDate;
            this.ViolationTicketNumber = model.ViolationTicketNumber;
            this.ViolationTime = model.ViolationTime;

            this.CountOneDescription = model.Offences[0].OffenceDescription;
            this.CountOneAction = model.Offences[0].OffenceAgreementStatus;
            this.CountOneWillAppear = model.Offences[0].ReductionAppearInCourt.Value;
            this.CountOneAmount = model.Offences[0].AmountDue;

            if (model.Offences.Count >= 2)
            {
                this.CountTwoDescription = model.Offences[1].OffenceDescription;
                this.CountTwoAction = model.Offences[1].OffenceAgreementStatus;
                this.CountTwoAmount = model.Offences[1].AmountDue;
                this.CountTwoWillAppear = model.Offences[1].ReductionAppearInCourt.Value;
            }

            if (model.Offences.Count == 3)
            {
                this.CountThreeDescription = model.Offences[2].OffenceDescription;
                this.CountThreeAction = model.Offences[2].OffenceAgreementStatus;
                this.CountThreeAmount = model.Offences[2].AmountDue;
                this.CountThreeWillAppear = model.Offences[2].ReductionAppearInCourt.Value;
            }

            this.RequireInterpreter = model.Additional.InterpreterRequired;
            this.InterpreterLanguage = model.Additional.InterpreterLanguage;
            this.NumberofWitnesses = model.Additional.NumberOfWitnesses ?? 0;
            this.CallWitness = model.Additional.WitnessPresent;
        }

        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }
    
        public string CountOneAction { get; set; }
        public string CountTwoAction { get; set; }
        public string CountThreeAction { get; set; }

        public string CountOneDescription { get; set; } 
        public string CountTwoDescription { get; set; }
        public string CountThreeDescription { get; set; }

        public bool CountOneWillAppear { get; set; }
        public bool CountTwoWillAppear { get; set; }
        public bool CountThreeWillAppear { get; set; }
    

        public bool RequireInterpreter { get; set; }
        public string InterpreterLanguage { get; set; }
        public bool CallWitness { get; set; }

        public string LogoImage { get; set; }
        public int NumberofWitnesses { get; set; } 
        public decimal CountOneAmount { get; set; }
        public decimal CountTwoAmount { get; set; }
        public decimal CountThreeAmount { get; set; }
        public string DiscountDueDate { get; set; }//null or has valid or invalid value
        public decimal DiscountAmount { get; set; }//25 always
        public string ConfirmationNumber { get; set; }
    }
}
