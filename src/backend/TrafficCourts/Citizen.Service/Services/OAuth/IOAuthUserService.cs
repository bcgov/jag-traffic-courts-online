using TrafficCourts.Citizen.Service.Models.OAuth;

namespace TrafficCourts.Citizen.Service.Services
{
    public interface IOAuthUserService
    {
        /// <summary>
        /// Get user information
        /// </summary>
        /// <typeparam name="UserInfo"></typeparam>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TrafficCourts.Citizen.Service.Models.OAuth.UserInfo?> GetUserInfoAsync<UserInfo>(string token, CancellationToken cancellationToken);
    }
}
