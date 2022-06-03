namespace TrafficCourts.Common.Features.Lookups
{
    /// <summary>
    /// A cached look up service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICachedLookupService<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> GetListAsync();
    }
}
