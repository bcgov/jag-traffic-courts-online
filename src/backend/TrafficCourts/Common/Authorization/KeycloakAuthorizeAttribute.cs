using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class KeycloakAuthorizeAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="scope"></param>
    public KeycloakAuthorizeAttribute(string resource, string scope)
    {
        if (string.IsNullOrEmpty(resource)) throw new ArgumentException("Resource cannot be null or empty", nameof(resource));
        if (string.IsNullOrEmpty(scope)) throw new ArgumentException("Scope cannot be null or empty", nameof(scope));

        Resource = resource;
        Scope = scope;

        Policy = $"{Resource}#{Scope}";
    }

    public string Resource { get; set; }
    public string Scope { get; set; }

    public override int GetHashCode()
    {
        return Policy!.GetHashCode();
    }
}
