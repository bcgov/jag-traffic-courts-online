# Dispute  API

This directory contains application source code

| app | url |
| --- | --- |
| Dispute API Swagger UI | [@baseurl/swagger](http://localhost:5050/swagger) |
| Dispute API OpenApi Specification | [@baseurl/swagger/v1/swagger.json](http://localhost:5050/swagger/v1/swagger.json) |
| Dispute API Health | [@baseurl/health](http://localhost:5060/health) |

# Configuration
All configuration uses standard the 
[Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0) sources. 
Configuration sources are layered with the lower layers overriding any high level sources.  The default layers are: 

* appsettings.json (required)
* appsettings.*ASPNETCORE_ENVIRONMENT*.json (optional)
* [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows) (optional) only if ASPNETCORE_ENVIRONMENT = "Development"
* Environment Variables (optional)
* command line arguments  (optional)

Review [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0) for how the configuration works.
See section [Environment variables](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#environment-variables) for how configuration
is mapped to environment variables.

## Logging Configuration

Logging is configured in the main program.

| Configuration Key | Environment Variable | Description |
| ---| --- | --- |
| Splunk:Url | SPLUNK__URL | The Splunk HTTP Event Collector (HEC) url, ie https://localhost:8088/services/collector/event |
| Splunk:Token | SPLUNK__TOKEN | The HTTP Event Collector (HEC) token 

See [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration) for examples on how to configure Serilog using 
the [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0) sources.
If you are having troubles with your logging configuration and  Serilog is not behaving as you expect, 
see Serilog [Debugging and Diagnostics](https://github.com/serilog/serilog/wiki/Debugging-and-Diagnostics).

Example that can be placed in appsettings.Development.json to use debug level logging and also write to the Debug window.

```json
{
  "Serilog": {
    "MinimumLevel": "Debug",
    "WriteTo": [
      "Debug"
    ]
  }
}
```