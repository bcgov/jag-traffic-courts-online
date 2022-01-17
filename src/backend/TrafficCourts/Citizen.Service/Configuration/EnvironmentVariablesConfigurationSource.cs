namespace TrafficCourts.Citizen.Service.Configuration;

public class EnvironmentVariablesConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EnvironmentVariablesConfigurationProvider();
    }
}
