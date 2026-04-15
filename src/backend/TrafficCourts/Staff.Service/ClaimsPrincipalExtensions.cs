using System.Security.Claims;

namespace TrafficCourts.Staff.Service
{
    /// <summary>
    /// Extension methods for <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns a staff member's username.
        /// </summary>
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user?.Identity?.Name ?? string.Empty;
        }
    }
}
