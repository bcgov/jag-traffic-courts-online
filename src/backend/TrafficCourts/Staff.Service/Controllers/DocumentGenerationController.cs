using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

public class DocumentGenerationController : StaffControllerBase<DocumentGenerationController>
{
    private readonly IPrintDigitalCaseFileService _printService;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="service"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public DocumentGenerationController(IPrintDigitalCaseFileService service, ILogger<DocumentGenerationController> logger) : base(logger)
    {
        _printService = service ?? throw new ArgumentNullException(nameof(service));
    }


 

}
