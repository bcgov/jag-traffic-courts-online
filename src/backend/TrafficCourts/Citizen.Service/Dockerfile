#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#FROM registry.access.redhat.com/ubi8/dotnet-60-runtime:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#FROM registry.access.redhat.com/ubi8/dotnet-60:6.0 AS build
WORKDIR /src
COPY ["Citizen.Service/TrafficCourts.Citizen.Service.csproj", "Citizen.Service/"]
COPY ["Common/TrafficCourts.Common.csproj", "Common/"]
COPY ["Messaging/TrafficCourts.Messaging.csproj", "Messaging/"]
RUN dotnet restore "Citizen.Service/TrafficCourts.Citizen.Service.csproj"
COPY . .
WORKDIR "/src/Citizen.Service"
RUN dotnet build "TrafficCourts.Citizen.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrafficCourts.Citizen.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrafficCourts.Citizen.Service.dll"]