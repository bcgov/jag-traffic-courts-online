using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using AutoMapper;

namespace TrafficCourts.Workflow.Service.Services
{
    public class FileHistoryService : IFileHistoryService
    {
        private readonly ILogger<FileHistoryService> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;
        private readonly IMapper _mapper;


        public FileHistoryService(ILogger<FileHistoryService> logger, IOracleDataApiService oracleDataApiService, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Saves file history
        /// </summary>
        /// <param name="fileHistoryRecord"></param>
        /// <returns>returns id of new file history record</returns>
        public async Task<long> SaveFileHistoryAsync(SaveFileHistoryRecord fileHistoryRecord, CancellationToken cancellationToken)
        {
            try
            {
                // prepare file history record
                if (!string.IsNullOrEmpty(fileHistoryRecord.NoticeOfDisputeId))
                {
                    Guid guid = new(fileHistoryRecord.NoticeOfDisputeId);
                    Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(guid, cancellationToken);
                    if (dispute != null)
                    {
                        fileHistoryRecord.DisputeId = dispute.DisputeId;
                        fileHistoryRecord.TicketNumber = dispute.TicketNumber;
                    }
                }
                else if (!string.IsNullOrEmpty(fileHistoryRecord.TicketNumber))
                {
                    JJDispute dispute = await _oracleDataApiService.GetJJDisputeAsync(fileHistoryRecord.TicketNumber, false, cancellationToken);
                    fileHistoryRecord.DisputeId = dispute.OccamDisputeId;
                    fileHistoryRecord.NoticeOfDisputeId = dispute.NoticeOfDisputeGuid;
                }
                FileHistory fileHistory = _mapper.Map<FileHistory>(fileHistoryRecord);
                long id = await _oracleDataApiService.CreateFileHistoryAsync(fileHistory, cancellationToken);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception saving file history.");
                throw;
            }
        }
    }
}
