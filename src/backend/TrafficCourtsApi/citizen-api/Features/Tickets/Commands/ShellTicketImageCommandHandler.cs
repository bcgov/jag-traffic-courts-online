using Gov.CitizenApi.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class ShellTicketImageCommand : ShellTicketImage, IRequest<ShellTicketImageResponse>
    {
    }

    public class ShellTicketImageResponse
    {
    }

    public class ShellTicketImageCommandHandler : IRequestHandler<ShellTicketImageCommand, ShellTicketImageResponse>
    {
        private readonly ILogger _logger;
        private IWebHostEnvironment _hostEnvironment;

        public ShellTicketImageCommandHandler(ILogger<ShellTicketImageCommandHandler> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ShellTicketImageResponse> Handle(ShellTicketImageCommand shellTicketImage, CancellationToken cancellationToken)
        {
            //we definitely won't save the image in app disk. So, following code will be discarded. no need to wrtie unit test.
            if (shellTicketImage.Image != null && shellTicketImage.Image.Length > 0)
            {
                string rootPath = _hostEnvironment.ContentRootPath;
                var pathWithFolder = Path.Combine(rootPath, "imageData");
                //create dir
                if (!Directory.Exists(pathWithFolder))
                {
                    Directory.CreateDirectory(pathWithFolder);
                }

                //save
                string[] fileNames = shellTicketImage.Image.FileName?.Split(".");
                string fileName = shellTicketImage.ViolationTicketNumber + ((fileNames.Length >= 2) ? "."+fileNames[1]?.ToLower(): string.Empty);
                string fullPath = Path.Combine(pathWithFolder, fileName);
                _logger.LogDebug($"fullPath ={fullPath}");
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await shellTicketImage.Image.CopyToAsync(fileStream);
                }

            }
            return new ShellTicketImageResponse();           
        }

    }
}
