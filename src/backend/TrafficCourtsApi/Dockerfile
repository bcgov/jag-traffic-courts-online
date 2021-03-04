# FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
FROM registry.redhat.io/rhel8/dotnet-50-runtime AS base
WORKDIR /opt/app-root
EXPOSE 80

# FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
FROM registry.redhat.io/rhel8/dotnet-50 AS build

WORKDIR /src/
COPY . .
RUN dotnet restore "DisputeApi.Web/DisputeApi.Web.csproj"

RUN dotnet build "DisputeApi.Web/DisputeApi.Web.csproj" -c Release -o /opt/app-root

FROM build AS publish
RUN dotnet publish "DisputeApi.Web/DisputeApi.Web.csproj" -c Release -o /opt/app-root

FROM base AS final
COPY --from=publish /opt/app-root .

ENTRYPOINT ["sh","-c","dotnet DisputeApi.Web.dll"]
