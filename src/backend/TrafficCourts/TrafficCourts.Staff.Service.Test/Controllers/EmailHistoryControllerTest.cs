using FluentAssertions;
using FluentAssertions.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

public class EmailHistoryControllerTest
{
    
    [Fact]
    public async void TestGetEmailHistory200Result()
    {
        // Mock the IEmailHistoryService to return a couple File history records, confirm controller returns them.

        // Arrange
        EmailHistory emailHistory1 = new();
        emailHistory1.EmailHistoryId = 1;
        emailHistory1.TicketNumber = "TestTicket01";
        EmailHistory emailHistory2 = new();
        emailHistory2.EmailHistoryId =2;
        emailHistory2.TicketNumber = "TestTicket01";
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

    [Fact]
    public void AllEndpointsShouldImplementAuthorizeAttribute()
    {
        // Check all endpoints of DisputeController to confirm all are guarded with proper KeycloakAuthorization or explicit AllowAnonymous Attribute

        // Arrange
        var _endpoints = new List<(Type, MethodInfo)>(); // All endpoints to check in DisputeController

        var assembly = Assembly.GetAssembly(typeof(EmailHistoryController));
        var allControllers = AllTypes.From(assembly).ThatDeriveFrom<StaffControllerBase<EmailHistoryController>>();

        foreach (Type t in allControllers)
        {
            var mInfos = t.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.DeclaringType.Equals(t)).ToList();
            foreach (MethodInfo mInfo in mInfos)
                _endpoints.Add((t, mInfo));
        }

        // Act
        var endpointsWithoutAuthorizeAttribute = _endpoints.Where(t => !t.Item2.IsDefined(typeof(KeycloakAuthorizeAttribute), false) && !t.Item2.IsDefined(typeof(AllowAnonymousAttribute), false)).ToList();
        var brokenEndpoints = string.Join(" and ", endpointsWithoutAuthorizeAttribute.Select(x => x.Item2.Name));

        // Assert
        endpointsWithoutAuthorizeAttribute.Count.Should().Be(0, "because {0} should have the KeycloakAuthorization or Anonymous attribute", brokenEndpoints);
    }
}
