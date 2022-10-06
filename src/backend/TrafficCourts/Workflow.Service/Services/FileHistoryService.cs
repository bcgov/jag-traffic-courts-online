using Microsoft.Extensions.Options;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using AutoMapper;
using MimeKit;
using MimeKit.Text;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Workflow.Service.Configuration;

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
            _oracleDataApiService = oracleDataApiService;
            _mapper = mapper;
        }

        /// <summary>
        /// Saves file history
        /// </summary>
        /// <param name="fileHistoryRecord"></param>
        /// <returns>returns id of new file history record</returns>
        public async Task<long> SaveFileHistoryAsync(FileHistoryRecord fileHistoryRecord, CancellationToken cancellationToken)
        {
            try
            {
                // prepare file history record
                FileHistory fileHistory = _mapper.Map<FileHistory>(fileHistoryRecord);
                long Id = await _oracleDataApiService.CreateFileHistoryAsync(fileHistory);
                return Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception saving file history.");
                throw;
            }
        }
    }
}
