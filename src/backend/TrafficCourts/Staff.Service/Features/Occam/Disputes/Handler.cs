using MediatR;
using System.Text;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Occam;
using TrafficCourts.OrdsDataService.Tco;
using TrafficCourts.Staff.Service.Features.CourtFiles.Summaries;

namespace TrafficCourts.Staff.Service.Features.Occam.Disputes;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IOccamDisputeRepository _repository;

    public Handler(IOccamDisputeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        // todo: add parameters to the request

        var items = await _repository.GetListAsync(null, cancellationToken);

        var buffer = new StringBuilder();
        List<Domain.Models.DisputeCaseFileSummary> models = items.Select(_ => _.ToDomainModel()).ToList();

        // Add these to the request instead, and do them DB side?
        if (request.Parameters != null)
        {
            // Do filtering? Why aren't we doing this DB side??!
        }

        return new Response(models);
    }

    private Response CreateResponse(OrdsDataService.OrdsDataServicePagedCollectionResponse<OrdsDisputeCaseFileSummary> pagedCollection)
    {
        if (pagedCollection.Rows is not null)
        {
            var items = pagedCollection.Rows.Select(Map);

            var offset = pagedCollection.Offset;
            var pageSize = pagedCollection.Fetch;
            var totalRows = pagedCollection.TotalRows;

            int pageNumber = (offset / pageSize) + 1;
            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

            var pagedList = new PagedDisputeCaseFileSummaryCollection(items, pageNumber, pageSize, pagedCollection.TotalRows);
            return new Response(pagedList);
        }

        // generate an error id that we log and return to the client
        string errorId = Guid.NewGuid().ToString("n");

        var error = pagedCollection.Errors?.FirstOrDefault();
        var logger = _logger.ForContext("ErrorId", errorId);

        if (error is not null)
        {
            logger
                .ForContext("ErrorCode", error.ErrorCode)
                .ForContext("ErrorMessage", error.ErrorMessage)
                .ForContext("ErrorStack", error.ErrorStack)
                .Error("Error fetching data from ORDS");
        }
        else
        {
            logger
                .Warning("No data returned from ORDS, no error details are available");
        }

        return new Response(errorId);
    }
}
