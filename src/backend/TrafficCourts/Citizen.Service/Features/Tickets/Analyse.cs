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
                this.Image = image ?? throw new ArgumentNullException(nameof(image));
            }

            public IFormFile Image { get; set; }
        }

        public class AnalyseResponse
        {
            public AnalyseResponse(ViolationTicket violationTicket)
            {
                this.ViolationTicket = violationTicket;
            }

            public ViolationTicket? ViolationTicket { get; set; }
        }

        public class Handler : IRequestHandler<AnalyseRequest, AnalyseResponse>
        {
            private readonly ILogger<Handler> _logger;
            private readonly string _apiKey;
            private readonly Uri _endpoint;

            public Handler(ILogger<Handler> logger, IOptions<FormRecognizerConfigurationOptions> options)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _apiKey = String.IsNullOrEmpty(options.Value.ApiKey) ? throw new ArgumentNullException(nameof(options.Value.ApiKey)) : options.Value.ApiKey;
                _endpoint = options.Value.Endpoint ?? throw new ArgumentNullException(nameof(options.Value.Endpoint));
            }

            public async Task<AnalyseResponse> Handle(AnalyseRequest request, CancellationToken cancellationToken)
            {
                _logger.LogDebug("Analysing {FileName}", request.Image.FileName);

                AzureKeyCredential credential = new AzureKeyCredential(_apiKey);
                DocumentAnalysisClient client = new DocumentAnalysisClient(_endpoint, credential);

                using Stream stream = GetImageStream(request.Image);
                AnalyzeDocumentOperation analyseDocumentOperation = await client.StartAnalyzeDocumentAsync("ViolationTicket", stream, null, cancellationToken);
                await analyseDocumentOperation.WaitForCompletionAsync();

                AnalyzeResult result = analyseDocumentOperation.Value;

                // For some reason the Azure.AI.FormRecognizer.DocumentAnalysis.BoundingBoxes are not serialized (always null in JSON)
                // return new Response(result.Documents[0].Fields);

                // Return a custom mapping of DocumentFields to a structured object for validation and serialization.
                ViolationTicket violationTicket = Map(result);
                AnalyseResponse response = new AnalyseResponse(violationTicket);
                return response;
            }

            /// Returns a Stream for the given image
            private Stream GetImageStream(IFormFile image)
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

            /// Maps AnalyzeResult to ViolationTicket
            private ViolationTicket Map(AnalyzeResult result)
            {
                // Iterate over results, creating a structured document
                ViolationTicket violationTicket = new ViolationTicket();
                foreach (AnalyzedDocument document in result.Documents)
                {
                    violationTicket.Confidence = document.Confidence;

                    if (document.Fields is not null)
                    {
                        foreach (KeyValuePair<String, DocumentField> pair in document.Fields)
                        {
                            ViolationTicket.Field field = new ViolationTicket.Field();
                            field.Name = pair.Key;
                            field.Value = pair.Value.Content;
                            field.Confidence = pair.Value.Confidence;
                            field.Type = Enum.GetName(pair.Value.ValueType);
                            foreach (BoundingRegion region in pair.Value.BoundingRegions)
                            {
                                ViolationTicket.BoundingBox boundingBox = new ViolationTicket.BoundingBox();
                                boundingBox.Points.Add(new ViolationTicket.Point(region.BoundingBox[0].X, region.BoundingBox[0].Y));
                                boundingBox.Points.Add(new ViolationTicket.Point(region.BoundingBox[1].X, region.BoundingBox[1].Y));
                                boundingBox.Points.Add(new ViolationTicket.Point(region.BoundingBox[2].X, region.BoundingBox[2].Y));
                                boundingBox.Points.Add(new ViolationTicket.Point(region.BoundingBox[3].X, region.BoundingBox[3].Y));
                                field.BoundingBoxes.Add(boundingBox);
                            }

                            violationTicket.Fields.Add(field);
                        }
                    }
                }
                return violationTicket;
            }
        }
    }
}
