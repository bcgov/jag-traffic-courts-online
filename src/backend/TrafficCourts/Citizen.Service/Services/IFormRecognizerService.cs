using Azure.AI.FormRecognizer.DocumentAnalysis;
using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services;

public interface IFormRecognizerService
{

    /// <summary>Analyses the specified image, extracting all text via OCR</summary>
    public Task<AnalyzeResult> AnalyzeImageAsync(MemoryStream stream, CancellationToken cancellationToken);

    /// <summary>Maps an AnalyzeResult to an OcrViolationTicket object</summary>
    public OcrViolationTicket Map(AnalyzeResult result);

}
