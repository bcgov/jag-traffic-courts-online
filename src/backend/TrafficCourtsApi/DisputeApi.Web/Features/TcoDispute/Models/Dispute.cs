using System.ComponentModel.DataAnnotations;

namespace DisputeApi.Web.Features.TcoDispute.Models
{
    public class Dispute
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string EmailAddress { get; set; }
        public bool LawyerPresent { get; set; }
        public bool InterpreterRequired { get; set; }
        public string InterpreterLanguage { get; set; }
        public bool CallWitness { get; set; }
        public bool CertifyCorrect { get; set; }
        public string StatusCode { get; set; }
    }
}
