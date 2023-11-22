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
