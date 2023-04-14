using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SearchDispute message, which will return dispute Id and status
    /// </summary>
    public class SearchDisputeConsumer : IConsumer<SearchDisputeRequest>
    {
        private readonly ILogger<SearchDisputeConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;

        public SearchDisputeConsumer(ILogger<SearchDisputeConsumer> logger, IOracleDataApiService oracleDataApiService)
        {
            _logger = logger;
            _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        }

        /// <summary>
        /// Consumer search for the dispute
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<SearchDisputeRequest> context)
        {
            using var scope = _logger.BeginConsumeScope(context);

            string? ticketNumber = context.Message.TicketNumber;
            string? issuedTime = context.Message.IssuedTime;
            Guid? noticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid;

            try
            {
                IList<DisputeResult> searchResult = await _oracleDataApiService.SearchDisputeAsync(ticketNumber, issuedTime, noticeOfDisputeGuid, context.CancellationToken)
                    .ConfigureAwait(false);

                if (searchResult.Count == 0)
                {
                    _logger.LogDebug("No results found, returning not found");
                    await context.RespondAsync(SearchDisputeResponse.NotFound);
                    return;
                }

                DisputeResult result = searchResult.Count == 1
                    ? searchResult[0]
                    : searchResult.OrderByDescending(d => d.DisputeId).First();

                if (string.IsNullOrEmpty(result.NoticeOfDisputeGuid))
                {
                    _logger.LogDebug("Last created dispute does not have a NoticeOfDisputeId, returning not found");
                    await context.RespondAsync(SearchDisputeResponse.NotFound);
                    return;
                }

                if (!Guid.TryParse(result.NoticeOfDisputeGuid, out Guid guid))
                {
                    // TODO: Add result.NoticeOfDisputeGuid value to log properties 
                    _logger.LogDebug("Last created dispute does not have a valid NoticeOfDisputeId value, returning not found");
                    await context.RespondAsync(SearchDisputeResponse.NotFound);
                    return;
                }

                await context.RespondAsync(new SearchDisputeResponse()
                {
                    NoticeOfDisputeGuid = guid,
                    DisputeStatus = result?.DisputeStatus.ToString(),
                    JJDisputeStatus = result?.JjDisputeStatus?.ToString(),
                    HearingType = result?.JjDisputeHearingType?.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                await context.RespondAsync(SearchDisputeResponse.Error);
            }
        }
    }
}
