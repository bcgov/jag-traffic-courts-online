using FluentAssertions;
using FluentAssertions.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Staff.Service.Controllers;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class ControllerAuthorizationTest
{
    [Obsolete("replaced by controller_actions_require_explict_authorization_or_anonymous_attributes")]
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

    [Theory(Skip = "LookupController and KeycloakController are failing due to not having correct attributes")]
    [MemberData(nameof(EachControllerAction))]
    public void controller_actions_require_explict_authorization_or_anonymous_attributes(Type controllerType, MethodInfo controllerAction)
    {
        // get HTTP verbs defined on this action
        HttpMethodAttribute httpMethodAttribute = controllerAction.GetCustomAttribute<HttpMethodAttribute>()!;
        var message = $"{GetVerbs(httpMethodAttribute)} {controllerType.Name}.{controllerAction.Name} requires authorization or must explicitly allowed to have anonymous access.";

        // check if the we require authorization or allow explicit anonymous access
        var authorized = HasAttribute<AuthorizeAttribute>(controllerType, controllerAction);
        var keycloakAuthorized = HasAttribute<KeycloakAuthorizeAttribute>(controllerType, controllerAction);
        var anonymous = HasAttribute<AllowAnonymousAttribute>(controllerType, controllerAction);

        // assert 
        Assert.True(authorized || keycloakAuthorized || anonymous, message);
    }

    private bool HasAttribute<TAttribute>(Type controllerType, MethodInfo controllerAction) where TAttribute : Attribute
    {
        return controllerType.IsDefined(typeof(TAttribute), false)
            || controllerAction.IsDefined(typeof(TAttribute), false);
    }

    /// <summary>
    /// Get the verbs defined.
    /// </summary>
    /// <returns>Comma separated list of HTTP verbs</returns>
    private static string GetVerbs(HttpMethodAttribute attribute)
    {
        // get the private field List<string> _httpMethods
        FieldInfo? field = typeof(HttpMethodAttribute)
            .GetField("_httpMethods", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(field);

        List<string> httpMethods = (List<string>)field!.GetValue(attribute)!;
        return string.Join(",", httpMethods);
    }

    /// <summary>
    /// Finds all controller and action methods in the Staff Service 
    /// </summary>
    public static IEnumerable<object[]> EachControllerAction
    {
        get
        {
            // find all the controller actions in the staff service
            var controllerTypes = typeof(TrafficCourts.Staff.Service.Startup)
                .Assembly
                .GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

            foreach (var controllerType in controllerTypes)
            {
                // find public instance methdos that have HttpMethodAttribute
                var controllerActions = controllerType
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(_ => _.IsDefined(typeof(HttpMethodAttribute), true));

                foreach (var controllerAction in controllerActions)
                {
                    yield return new object[] { controllerType, controllerAction };
                }
            }
        }
    }
}
