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
