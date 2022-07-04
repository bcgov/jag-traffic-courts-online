using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services;

public interface IFormRecognizerService
{

    /// <summary>Analyses the specified image, extracting all text via OCR and mapping results to an OcrViolationTicket.</summary>
    public Task<OcrViolationTicket> AnalyzeImageAsync(MemoryStream stream, CancellationToken cancellationToken);

}
