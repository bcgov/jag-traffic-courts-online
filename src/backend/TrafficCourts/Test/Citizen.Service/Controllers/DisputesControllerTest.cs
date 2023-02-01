using AutoMapper;
using HashidsNet;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Threading;
using TrafficCourts.Citizen.Service.Controllers;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Common.Features.EmailVerificationToken;
using TrafficCourts.Coms.Client;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Controllers
{
    public class DisputesControllerTest
    {
        [Fact]
        public async void TestDownloadDocument200Result()
        {
            // Arrange
            var mockTicketDispute = new Mock<NoticeOfDispute>();
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<DisputesController>>();
            var mockBus = new Mock<IBus>();
            var mockHashids = new Mock<IHashids>();
            var mockOAuthService = new Mock<IOAuthUserService>();
            var mockMapper = new Mock<IMapper>();
            var mockComsService = new Mock<ICitizenDocumentService>();
            var mockControllerContext = new Mock<ControllerContext>();

            var tokenEncoder = Mock.Of<IDisputeEmailVerificationTokenEncoder>();

            // Mock authentication and token
            var token = "token";
            var context = new Mock<HttpContext>();
            context.Setup(c => c.User.Identity!.IsAuthenticated).Returns(true);
            context.Setup(c => c.Request.Headers.Authorization).Returns(token);
            
            mockControllerContext.Object.HttpContext = context.Object;

            var disputeController = new DisputesController(mockBus.Object, mockMediator.Object, mockLogger.Object, mockHashids.Object, tokenEncoder, mockOAuthService.Object, mockMapper.Object, mockComsService.Object);
            disputeController.ControllerContext = mockControllerContext.Object;

            var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
            Coms.Client.File mockFile = new(fileStream, "testFile");
            Guid guid = Guid.NewGuid();
            mockFile.Metadata.Add("ticket-number", "AO38375804");
            mockFile.Metadata.Add("virus-scan-status", "clean");
            var filename = mockFile.FileName;
            mockComsService
                .Setup(_ => _.GetFileAsync(guid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockFile);

            // Act
            var result = await disputeController.DownloadDocumentAsync(guid, CancellationToken.None);

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(filename, fileResult.FileDownloadName);
        }

        [Fact]
        public async void TestDownloadDocument401UnauthorizedResult()
        {
            // Arrange
            var mockTicketDispute = new Mock<NoticeOfDispute>();
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<DisputesController>>();
            var mockBus = new Mock<IBus>();
            var mockHashids = new Mock<IHashids>();
            var mockOAuthService = new Mock<IOAuthUserService>();
            var mockMapper = new Mock<IMapper>();
            var mockComsService = new Mock<ICitizenDocumentService>();
            var mockControllerContext = new Mock<ControllerContext>();

            var tokenEncoder = Mock.Of<IDisputeEmailVerificationTokenEncoder>();

            // Mock authentication and token
            var token = "";
            var context = new Mock<HttpContext>();
            context.Setup(c => c.User.Identity!.IsAuthenticated).Returns(false);
            context.Setup(c => c.Request.Headers.Authorization).Returns(token);

            mockControllerContext.Object.HttpContext = context.Object;

            var disputeController = new DisputesController(mockBus.Object, mockMediator.Object, mockLogger.Object, mockHashids.Object, tokenEncoder, mockOAuthService.Object, mockMapper.Object, mockComsService.Object);
            disputeController.ControllerContext = mockControllerContext.Object;

            var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
            Coms.Client.File mockFile = new(fileStream, "testFile");
            Guid guid = Guid.NewGuid();
            mockFile.Metadata.Add("ticket-number", "AO38375804");
            mockFile.Metadata.Add("virus-scan-status", "clean");
            var filename = mockFile.FileName;
            mockComsService
                .Setup(_ => _.GetFileAsync(guid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockFile);

            // Act
            var result = await disputeController.DownloadDocumentAsync(guid, CancellationToken.None);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal((int)HttpStatusCode.Unauthorized, problemDetails.Status);
            Assert.True(problemDetails?.Title?.Contains("Exception Authorizing User"));
        }

        [Fact]
        public async void TestDownloadDocumentMissingMetadataKeyThrowsObjectManagementServiceException500result()
        {
            // Arrange
            var mockTicketDispute = new Mock<NoticeOfDispute>();
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<DisputesController>>();
            var mockBus = new Mock<IBus>();
            var mockHashids = new Mock<IHashids>();
            var mockOAuthService = new Mock<IOAuthUserService>();
            var mockMapper = new Mock<IMapper>();
            var mockComsService = new Mock<ICitizenDocumentService>();
            var mockControllerContext = new Mock<ControllerContext>();

            var tokenEncoder = Mock.Of<IDisputeEmailVerificationTokenEncoder>();

            // Mock authentication and token
            var token = "token";
            var context = new Mock<HttpContext>();
            context.Setup(c => c.User.Identity!.IsAuthenticated).Returns(true);
            context.Setup(c => c.Request.Headers.Authorization).Returns(token);

            mockControllerContext.Object.HttpContext = context.Object;

            var disputeController = new DisputesController(mockBus.Object, mockMediator.Object, mockLogger.Object, mockHashids.Object, tokenEncoder, mockOAuthService.Object, mockMapper.Object, mockComsService.Object);
            disputeController.ControllerContext = mockControllerContext.Object;

            var fileStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes("FileData"));
            Coms.Client.File mockFile = new(fileStream, "testFile");
            Guid guid = Guid.NewGuid();
            mockFile.Metadata.Add("ticket-number", "AO38375804");
            var filename = mockFile.FileName;
            mockComsService
                .Setup(_ => _.GetFileAsync(guid, It.IsAny<CancellationToken>()))
                .Throws(new ObjectManagementServiceException(It.IsAny<string>()));

            // Act
            var result = await disputeController.DownloadDocumentAsync(guid, CancellationToken.None);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal((int)HttpStatusCode.InternalServerError, problemDetails.Status);
            Assert.True(problemDetails?.Title?.Contains("Error getting file from COMS"));
        }

        [Fact]
        public async void TestCreateDisputeOkResult()
        {
            // Arrange
            var mockTicketDispute = new Mock<NoticeOfDispute>();
            var mockMediator = new Mock<IMediator>();
            var mockLogger = new Mock<ILogger<DisputesController>>();
            var mockBus = new Mock<IBus>();
            var mockHashids = new Mock<IHashids>();
            var mockOAuthService = new Mock<IOAuthUserService>();
            var mockMapper = new Mock<IMapper>();
            var mockComsService = new Mock<ICitizenDocumentService>();

            var tokenEncoder = Mock.Of<IDisputeEmailVerificationTokenEncoder>();

            var disputeController = new DisputesController(mockBus.Object, mockMediator.Object, mockLogger.Object, mockHashids.Object, tokenEncoder, mockOAuthService.Object, mockMapper.Object, mockComsService.Object);
            var request = new Create.Request(mockTicketDispute.Object);
            var createResponse = new Create.Response();

            mockMediator
                .Setup(_ => _.Send<Create.Response>(It.IsAny<Create.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createResponse);

            // Act
            var result = await disputeController.CreateAsync(mockTicketDispute.Object, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
    }
}
