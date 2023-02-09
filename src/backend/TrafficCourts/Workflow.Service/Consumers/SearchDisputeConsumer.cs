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
            string? noticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid?.ToString();
            ICollection<DisputeResult> searchResult;
            try
            {
                searchResult = await _oracleDataApiService.SearchDisputeAsync(ticketNumber, issuedTime, noticeOfDisputeGuid, context.CancellationToken);
                if (searchResult is null || searchResult.Count == 0)
                {
                    throw new NullReferenceException("Dispute not found");
                }
                DisputeResult result = searchResult.OrderByDescending(d => d.DisputeId).First();
                if (!String.IsNullOrEmpty(result.NoticeOfDisputeGuid))
                {
                    _ = Guid.TryParse(result.NoticeOfDisputeGuid, out Guid guid);
                    await context.RespondAsync<SearchDisputeResponse>(new SearchDisputeResponse()
                    {
                        NoticeOfDisputeGuid = guid,
                        DisputeStatus = result?.DisputeStatus.ToString(),
                        JJDisputeStatus = result?.JjDisputeStatus?.ToString(),
                        HearingType = result?.JjDisputeHearingType?.ToString()
                    });
                }
                else
                {
                    throw new NullReferenceException("Dispute not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                await context.RespondAsync<SearchDisputeResponse>(new SearchDisputeResponse { IsError = true });
            }
        }
    }
}
