using TrafficCourts.Common.Features.FilePersistence;

namespace TrafficCourts.Citizen.Service.Configuration;

public static class Extensions
{
    /// <summary>
    /// Adds <see cref="IFilePersistenceService"/> that uses S3 Object Storage.s
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddObjectStorageFilePersistence(this WebApplicationBuilder builder)
    {        
        builder.Services.AddObjectStorageFilePersistence(builder.Configuration.GetSection("ObjectStorage"));
        return builder;
    }

    public static WebApplicationBuilder AddInMemoryFilePersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddInMemoryFilePersistence();
        return builder;
    }
}
