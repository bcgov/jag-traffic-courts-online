using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for GetDispute message, which will return dispute
    /// </summary>
    public class GetDisputeConsumer : IConsumer<GetDisputeRequest>
    {
        private readonly ILogger<GetDisputeConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;

        public GetDisputeConsumer(ILogger<GetDisputeConsumer> logger, IOracleDataApiService oracleDataApiService)
        {
            _logger = logger;
            _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        }

        /// <summary>
        /// Consumer for getting the dispute
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<GetDisputeRequest> context)
        {
            using var scope = _logger.BeginConsumeScope(context);
            Dispute? searchResult;
            try
            {
                searchResult = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(context.Message.NoticeOfDisputeGuid, context.CancellationToken);
                if (searchResult is null)
                {
                    searchResult = new Dispute();
                }
                await context.RespondAsync<SubmitNoticeOfDispute>(searchResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                await context.RespondAsync<SubmitNoticeOfDispute>(new SubmitNoticeOfDispute());
            }
        }
    }
}
