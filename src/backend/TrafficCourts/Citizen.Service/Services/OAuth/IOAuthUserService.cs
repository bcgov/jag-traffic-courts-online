using TrafficCourts.Citizen.Service.Models.OAuth;

namespace TrafficCourts.Citizen.Service.Services
{
    public interface IOAuthUserService
    {
        /// <summary>
        /// Get user information
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        UserInfo? GetUserInfo<UserInfo>(string token);
    }
}
