using TrafficCourts.Citizen.Service.Models.OAuth;

namespace TrafficCourts.Citizen.Service.Services
{
    public interface IOAuthUserService
    {
        /// <summary>
        /// Get user information
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The user info for the user authenticated with <paramref name="token"/>, or null.</returns>
        Task<Models.OAuth.UserInfo?> GetUserInfoAsync(string token, CancellationToken cancellationToken);
    }
}
