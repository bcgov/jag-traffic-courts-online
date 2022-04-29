namespace TrafficCourts.Citizen.Service.Services
{
    public interface IRedisCacheService
    {
        /// <summary>
        /// Generic Set method that saves or writes json serialized data that is associated to the provided key to Redis Cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        Task SetRecordAsync<T>(string key, T data, TimeSpan? expireTime);

        /// <summary>
        /// Generic Get method that returns deserialized json data for the provided key from Redis Cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T?> GetRecordAsync<T>(string key);
    }
}
