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
using TrafficCourts.Domain.Models;
using TrafficCourts.Staff.Service.Controllers;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class EmailHistoryControllerTest
{
    
    [Fact]
    public async void TestGetEmailHistory200Result()
    {
        // Mock the IEmailHistoryService to return a couple File history records, confirm controller returns them.

        // Arrange
        EmailHistory emailHistory1 = new();
        emailHistory1.EmailHistoryId = 1;
        emailHistory1.OccamDisputeId = 3L;
        EmailHistory emailHistory2 = new();
        emailHistory2.EmailHistoryId =2;
        emailHistory2.OccamDisputeId = 4L;
        List<EmailHistory> fileHistories = new() { emailHistory1, emailHistory2 };
        var EmailHistoryService = new Mock<IEmailHistoryService>();
        EmailHistoryService
            .Setup(_ => _.GetEmailHistoryForTicketAsync("TestTicket01", It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileHistories);
        var mockLogger = new Mock<ILogger<EmailHistoryController>>();
        EmailHistoryController EmailHistoryController = new(EmailHistoryService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await EmailHistoryController.GetEmailHistoryRecordsAsync("TestTicket01", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(2, ((List<EmailHistory>)okResult.Value).Count);
        Assert.Equal(emailHistory1, ((List<EmailHistory>)okResult.Value)[0]);
        Assert.Equal(emailHistory2, ((List<EmailHistory>)okResult.Value)[1]);
    }
}
