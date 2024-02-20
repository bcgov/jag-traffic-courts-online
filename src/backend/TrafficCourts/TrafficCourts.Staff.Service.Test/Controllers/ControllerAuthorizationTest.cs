using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrafficCourts.Common.Authorization;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Controllers;

public class ControllerAuthorizationTest
{
    [Theory]
    [MemberData(nameof(EachControllerAction))]
    public void controller_actions_require_explict_authorization_attributes(Type controllerType, MethodInfo controllerAction)
    {
        // get HTTP verbs defined on this action
        HttpMethodAttribute? httpMethodAttribute = controllerAction.GetCustomAttribute<HttpMethodAttribute>(true);
        Assert.NotNull(httpMethodAttribute); // MemberData EachControllerAction should filter on actions with this attribute

        var message = $"{GetVerbs(httpMethodAttribute)} {controllerType.Name}.{controllerAction.Name} requires an attribute AuthorizeAttribute or  KeycloakAuthorizeAttribute.";

        // check if the we require authorization or allow explicit anonymous access
        var authorized = HasAttribute<AuthorizeAttribute>(controllerType, controllerAction);
        var keycloakAuthorized = HasAttribute<KeycloakAuthorizeAttribute>(controllerType, controllerAction);

        // assert 
        Assert.True(authorized || keycloakAuthorized, message);
    }

    private bool HasAttribute<TAttribute>(Type controllerType, MethodInfo controllerAction) where TAttribute : Attribute
    {
        // look for TAttribute on this type or any of it's base class, but not on the controller action
        return controllerType.IsDefined(typeof(TAttribute), true);
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
            // Find all non abstract types that inherit from ControllerBase in the Service assembly
            var controllerTypes = typeof(TrafficCourts.Staff.Service.Startup)
                .Assembly
                .GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                .OrderBy(type => type.Name);

            foreach (var controllerType in controllerTypes)
            {
                // find public instance methdos that have HttpMethodAttribute
                var controllerActions = controllerType
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(method => method.IsDefined(typeof(HttpMethodAttribute), true))
                    .OrderBy(method => method.Name);

                foreach (var controllerAction in controllerActions)
                {
                    yield return new object[] { controllerType, controllerAction };
                }
            }
        }
    }
}
