using MassTransit;
using TrafficCourts.Domain.Models;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Interfaces;
using TrafficCourts.Workflow.Service.Mappers;
using System.Net;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SendEmail message.
    /// </summary>
    public class DisputantEmailSentConsumer : IConsumer<DisputantEmailSent>
    {
        private readonly IOracleDataApiService _oracleDataApiService;
        private readonly ILogger<DisputantEmailSentConsumer> _logger;

        public DisputantEmailSentConsumer(IOracleDataApiService oracleDataApiService, ILogger<DisputantEmailSentConsumer> logger)
        {
            _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<DisputantEmailSent> context)
        {
            using var scope = _logger.BeginConsumeScope(context);
            _logger.LogDebug("Calling email sent service");

            try
            {
                EmailHistory emailHistory = Mapper.ToEmailHistory(context.Message);

                await _oracleDataApiService.CreateEmailHistoryAsync(emailHistory, context.CancellationToken);
            } 
            catch (WebException ex)
            {
                HttpWebResponse? errorResponse = ex.Response as HttpWebResponse;
                if (errorResponse?.StatusCode == HttpStatusCode.NotFound || errorResponse?.StatusCode == HttpStatusCode.BadRequest)
                {
                    // dont requeue if dispute not found or bad request
                    _logger.LogError(ex.Message, context);
                }

            }
        }
    }
}
