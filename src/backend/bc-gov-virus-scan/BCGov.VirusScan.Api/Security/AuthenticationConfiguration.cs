namespace BCGov.VirusScan.Api.Security;

/// <summary>
/// Wrapper around the authentication configuration. In the future, 
/// this class will not be static. May optional configuration
/// </summary>
public static class AuthenticationConfiguration
{
    /// <summary>
    /// Should anonymous access be allowed?
    /// </summary>
    public const bool AllowAnonymous = true;
}
