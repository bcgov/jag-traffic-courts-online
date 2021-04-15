using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TrafficCourts.Common.Contract;

namespace DisputeWorker
{
    public class DisputeConsumer : IConsumer<IDispute>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DisputeConsumer> _logger;
        public DisputeConsumer(IServiceProvider serviceProvider, ILogger<DisputeConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IDispute> context)
        {
            try
            {
                var Message = context.Message;
                //var productService = _serviceProvider.GetService<IProductService>();
                //var product = await productService.Save(context.Message);

                //await productService.Publish(product);

                //await context.RespondAsync<ProductAccepted>(new
                //{
                //    Value = $"Received: {context.Message.MessageId}"
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductChangedConsumerError", ex);
            }

        }
    }

    public class Dispute : IDispute
    {
        public int Id { get; set; }
        public string ViolationTicketNumber { get; set; }
        public int OffenceNumber { get; set; }
        public string EmailAddress { get; set; }
        public OffenceAgreementStatus OffenceAgreementStatus { get; set; }
        public bool RequestReduction { get; set; }
        public bool RequestMoreTime { get; set; }
        public string ReductionReason { get; set; }
        public string MoreTimeReason { get; set; }
        public bool LawyerPresent { get; set; }
        public bool InterpreterRequired { get; set; }
        public bool WitnessPresent { get; set; }
        public string InterpreterLanguage { get; set; }
        public bool InformationCertified { get; set; }
        public DisputeStatus Status { get; set; }
    }
  
}
