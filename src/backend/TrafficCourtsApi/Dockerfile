
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build


WORKDIR /src
COPY . .
RUN dotnet restore "DisputeApi.Web/DisputeApi.Web.csproj"
RUN dotnet build "DisputeApi.Web/DisputeApi.Web.csproj" -c Release -o /opt/app-root
RUN dotnet publish "DisputeApi.Web/DisputeApi.Web.csproj" -c Release -o /opt/app-root

FROM registry.redhat.io/ubi8/dotnet-50-runtime AS base
WORKDIR /opt/app-root
EXPOSE 8080
COPY --from=build /opt/app-root .

ENTRYPOINT ["sh","-c","dotnet DisputeApi.Web.dll"]
