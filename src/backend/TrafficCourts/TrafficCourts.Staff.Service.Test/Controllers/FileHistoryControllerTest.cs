using FluentAssertions;
using FluentAssertions.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class FileHistoryControllerTest
{
    
    [Fact]
    public async void TestGetFileHistory200Result()
    {
        // Mock the IFileHistoryService to return a couple File history records, confirm controller returns them.

        // Arrange
        FileHistory fileHistory1 = new();
        fileHistory1.FileHistoryId = 1;
        fileHistory1.DisputeId = 3;
        FileHistory fileHistory2 = new();
        fileHistory2.FileHistoryId =2;
        fileHistory2.DisputeId = 4;
        List<FileHistory> fileHistories = new() { fileHistory1, fileHistory2 };
        var fileHistoryService = new Mock<IFileHistoryService>();
        fileHistoryService
            .Setup(_ => _.GetFileHistoryForTicketAsync("TestTicket01", It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileHistories);
        var mockLogger = new Mock<ILogger<FileHistoryController>>();
        FileHistoryController fileHistoryController = new(fileHistoryService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await fileHistoryController.GetFileHistoryRecordsAsync("TestTicket01", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(2, ((List<FileHistory>)okResult.Value).Count);
        Assert.Equal(fileHistory1, ((List<FileHistory>)okResult.Value)[0]);
        Assert.Equal(fileHistory2, ((List<FileHistory>)okResult.Value)[1]);
    }
}
