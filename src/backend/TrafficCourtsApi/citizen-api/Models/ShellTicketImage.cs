using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common;

namespace Gov.CitizenApi.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class ShellTicketImage
    {
        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public IFormFile Image{ get; set; }
    }
}
