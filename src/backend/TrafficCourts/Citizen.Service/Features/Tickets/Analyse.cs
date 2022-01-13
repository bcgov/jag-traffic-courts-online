using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using MediatR;
using Microsoft.Extensions.Options;
using TrafficCourts.Citizen.Service.Configuration;

namespace TrafficCourts.Citizen.Service.Features.Tickets
{
    public static class Analyse
    {
        public class AnalyseRequest : IRequest<AnalyseResponse>
        {
            public AnalyseRequest(IFormFile image)
            {
                this.image = image;
            }

            public IFormFile image { get; set; }
        }

        public class AnalyseResponse
        {
            public AnalyseResponse(ViolationTicket violationTicket)
            {
                this.violationTicket = violationTicket;
            }

            public ViolationTicket? violationTicket { get; set; }
        }

        public class Handler : IRequestHandler<AnalyseRequest, AnalyseResponse>
        {
            private readonly ILogger<Handler> _logger;
            private readonly String _apiKey;
            private readonly String _endpoint;

            public Handler(ILogger<Handler> logger, IOptions<FormRecognizerConfigurationOptions> options)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _apiKey = String.IsNullOrEmpty(options.Value.ApiKey) ? throw new ArgumentNullException(nameof(options.Value.ApiKey)) : options.Value.ApiKey;
                _endpoint = String.IsNullOrEmpty(options.Value.Endpoint) ? throw new ArgumentNullException(nameof(options.Value.Endpoint)) : options.Value.Endpoint;
            }

            public async Task<AnalyseResponse> Handle(AnalyseRequest request, CancellationToken cancellationToken)
            {
                _logger.LogDebug($"Analysing {request.image.FileName}");

                AzureKeyCredential credential = new AzureKeyCredential(_apiKey);
                DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(_endpoint), credential);

                using Stream stream = getImageStream(request.image);
                AnalyzeDocumentOperation analyseDocumentOperation = await client.StartAnalyzeDocumentAsync("ViolationTicket", stream);
                await analyseDocumentOperation.WaitForCompletionAsync();

                AnalyzeResult result = analyseDocumentOperation.Value;

                // For some reason the Azure.AI.FormRecognizer.DocumentAnalysis.BoundingBoxes are not serialized (always null in JSON)
                // return new Response(result.Documents[0].Fields);

                // Return a custom mapping of DocumentFields to a structured object for validation and serialization.
                ViolationTicket violationTicket = map(result);
                AnalyseResponse response = new AnalyseResponse(violationTicket);
                return response;
            }

            private Stream getImageStream(IFormFile image)
            {
                // Work around for a "System.ObjectDisposedException : Cannot access a closed file." error
                // - extract data from the file attachment to an in-memory byte[]
                // - create a new MemoryStream from the byte[]
                byte[] fileBytes;
                using (var fileStream = image.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                MemoryStream stream = new MemoryStream(fileBytes);
                return stream;
            }

            private ViolationTicket map(AnalyzeResult result)
            {
                // Iterate over results, creating a structured document
                ViolationTicket violationTicket = new ViolationTicket();
                foreach (AnalyzedDocument document in result.Documents)
                {
                    violationTicket.confidence = document.Confidence;

                    if (document.Fields != null)
                    {
                        foreach (KeyValuePair<String, DocumentField> pair in document.Fields)
                        {
                            ViolationTicket.Field field = new ViolationTicket.Field();
                            field.name = pair.Key;
                            field.value = pair.Value.Content;
                            field.confidence = pair.Value.Confidence;
                            field.type = Enum.GetName(pair.Value.ValueType);
                            foreach (BoundingRegion region in pair.Value.BoundingRegions)
                            {
                                ViolationTicket.BoundingBox boundingBox = new ViolationTicket.BoundingBox();
                                boundingBox.points.Add(new ViolationTicket.Point(region.BoundingBox[0].X, region.BoundingBox[0].Y));
                                boundingBox.points.Add(new ViolationTicket.Point(region.BoundingBox[1].X, region.BoundingBox[1].Y));
                                boundingBox.points.Add(new ViolationTicket.Point(region.BoundingBox[2].X, region.BoundingBox[2].Y));
                                boundingBox.points.Add(new ViolationTicket.Point(region.BoundingBox[3].X, region.BoundingBox[3].Y));
                                field.boundingBoxes.Add(boundingBox);
                            }

                            violationTicket.fields.Add(field);
                        }
                    }
                }
                return violationTicket;
            }
        }
    }
}
