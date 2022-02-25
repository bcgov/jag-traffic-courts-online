# dotnet-counters container
Creates container to run dotnet-counters in a side-car containing

## Images

| File | Description
| -- | -- |
| Dockerfile | uses Microsoft .NET 6 runtime image for use with local docker development |
| Dockerfile.rhel8 | uses the Universal Base Image 8 (ubi8) .NET 6 runtime image from Redhat for use on OpenShift |

## References
* [Collect diagnostics in containers](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/diagnostics-in-containers)
* [High CPU Usage with .NET Counters](https://youtu.be/7llxR-rH-gM)
* [Diagnosing thread pool exhaustion issues in .NET Core apps](https://youtu.be/isK8Cel3HP0)
