using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.TicketWorker.Models
{
    class DisputeEmail
    {
    public string ViolationTicketNumber { get; set; }
    public string ViolationTime { get; set; }
    public string ViolationDate { get; set; }
    
    public string CountOneAction { get; set; }
    public string CountTwoAction { get; set; }
    public string CountThreeAction { get; set; }

    public string CountOneDescription { get; set; }
    public string CountTwoDescription { get; set; }
    public string CountThreeDescription { get; set; }

    public string CountOneWillAppear { get; set; }
    public string CountTwoWillAppear { get; set; }
    public string CountThreeWillAppear { get; set; }
    

    public string RequireInterpreter { get; set; }
    public string InterpreterLanguage { get; set; }
    public string CallWitness { get; set; }

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
