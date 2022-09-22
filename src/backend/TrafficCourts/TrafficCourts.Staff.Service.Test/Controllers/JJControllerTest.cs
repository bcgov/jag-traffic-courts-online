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

public class JJControllerTest
{
    [Fact]
    public void AllEndpointsShouldImplementAuthorizeAttribute()
    {
        // Check all endpoints of JJDisputeController to confirm all are guarded with proper KeycloakAuthorization or explicit AllowAnonymous Attribute

        // Arrange
        var _endpoints = new List<(Type, MethodInfo)>(); // All endpoints to check in JJDisputeController

        var assembly = Assembly.GetAssembly(typeof(JJController));
        var allControllers = AllTypes.From(assembly).ThatDeriveFrom<VTCControllerBase<JJController>>();

        foreach (Type t in allControllers)
        {
            var mInfos = t.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.DeclaringType!.Equals(t)).ToList();
            foreach (MethodInfo mInfo in mInfos)
                _endpoints.Add((t, mInfo));
        }

        // Act
        var endpointsWithoutAuthorizeAttribute = _endpoints.Where(t => !t.Item2.IsDefined(typeof(KeycloakAuthorizeAttribute), false) && !t.Item2.IsDefined(typeof(AllowAnonymousAttribute), false)).ToList();
        var brokenEndpoints = string.Join(" and ", endpointsWithoutAuthorizeAttribute.Select(x => x.Item2.Name));

        // Assert
        endpointsWithoutAuthorizeAttribute.Count.Should().Be(0, "because {0} should have the KeycloakAuthorization or Anonymous attribute", brokenEndpoints);
    }

    [Fact]
    public async void TestAssignJJDisputesToJJ200Result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with ticket number (AJ201092461) to assign it to a JJ, confirm controller updates and assigns the JJ.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        List<string> ticketNumbers = new();
        ticketNumbers.Add(ticketnumber);
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.AssignJJDisputesToJJ(ticketNumbers, "Bruce Wayne", It.IsAny<CancellationToken>()));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.AssignJJDisputesToJJ(ticketNumbers, "Bruce Wayne", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async void TestAssignJJDisputesToJJT400Result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with no ticket number to assign it to a JJ, confirm controller returns 400 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "AJ201092461";
        dispute.TicketNumber = ticketnumber;
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.AssignJJDisputesToJJ(null, "Bruce Wayne", It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status400BadRequest, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.AssignJJDisputesToJJ(null, "Bruce Wayne", CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public async void TestAssignJJDisputesToJJT404Result()
    {
        // Mock the OracleDataApi to update a specific JJDispute with an invalid ticket number that is not exist in db to assign it to a JJ, confirm controller returns 404 when updating.

        // Arrange
        JJDispute dispute = new();
        string ticketnumber = "invalidTicketNum";
        dispute.TicketNumber = ticketnumber;
        List<string> ticketNumbers = new();
        ticketNumbers.Add(ticketnumber);
        var jjDisputeService = new Mock<IJJDisputeService>();
        jjDisputeService
            .Setup(_ => _.AssignJJDisputesToJJ(ticketNumbers, "Bruce Wayne", It.IsAny<CancellationToken>()))
            .Throws(new ApiException("msg", StatusCodes.Status404NotFound, "rsp", null, null));
        var mockLogger = new Mock<ILogger<JJController>>();
        JJController jjDisputeController = new(jjDisputeService.Object, mockLogger.Object);

        // Act
        IActionResult? result = await jjDisputeController.AssignJJDisputesToJJ(ticketNumbers, "Bruce Wayne", CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<HttpError>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }
}
