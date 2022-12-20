

using MassTransit;
using System.Text.Json;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Workflow.Service.Services;
using DisputantUpdateRequest = TrafficCourts.Messaging.MessageContracts.DisputantUpdateRequest;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputantUpdateRequestConsumer : IConsumer<DisputantUpdateRequest>
{
    private readonly ILogger<DisputantUpdateRequestConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;

    public DisputantUpdateRequestConsumer(ILogger<DisputantUpdateRequestConsumer> logger, IOracleDataApiService oracleDataApiService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
    }

    public async Task Consume(ConsumeContext<DisputantUpdateRequest> context)
    {
        _logger.LogDebug("Consuming message");
        DisputantUpdateRequest message = context.Message;

        Common.OpenAPIs.OracleDataApi.v1_0.DisputantUpdateRequest disputantUpdateRequest = new()
        {
            Status = DisputantUpdateRequestStatus2.PENDING,
            UpdateJson = JsonSerializer.Serialize(message)
        };

        if (message.EmailAddress is not null)
        {
            // TODO: Start email saga. TCVP-2009
        }

        // If some or all name fields have data, send a DISPUTANT_NAME update request
        if (!string.IsNullOrEmpty(message.DisputantGivenName1)
            || !string.IsNullOrEmpty(message.DisputantGivenName2)
            || !string.IsNullOrEmpty(message.DisputantGivenName3)
            || !string.IsNullOrEmpty(message.DisputantSurname)
            )
        {
            disputantUpdateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_NAME;
            await _oracleDataApiService.SaveDisputantUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputantUpdateRequest, context.CancellationToken);
        }

        // If some or all address fields have data, send a DISPUTANT_ADDRESS update request
        if (!string.IsNullOrEmpty(message.AddressLine1)
            || !string.IsNullOrEmpty(message.AddressLine2)
            || !string.IsNullOrEmpty(message.AddressLine3)
            || !string.IsNullOrEmpty(message.AddressCity)
            || !string.IsNullOrEmpty(message.AddressProvince)
            || !string.IsNullOrEmpty(message.PostalCode)
            || message.AddressProvinceCountryId is not null
            || message.AddressProvinceSeqNo is not null
            || message.AddressCountryId is not null
            )
        {
            disputantUpdateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_ADDRESS;
            await _oracleDataApiService.SaveDisputantUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputantUpdateRequest, context.CancellationToken);
        }

        // If some or all phone fields have data, send a DISPUTANT_PHONE update request
        if (message.HomePhoneNumber is not null) 
        {
            disputantUpdateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_PHONE;
            await _oracleDataApiService.SaveDisputantUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputantUpdateRequest, context.CancellationToken);
        }
    }
}
