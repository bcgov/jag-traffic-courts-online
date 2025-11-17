#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG BUILD_MODE=Release

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_MODE
WORKDIR /src
COPY ["BCGov.VirusScan.Api/BCGov.VirusScan.Api.csproj", "BCGov.VirusScan.Api/"]
RUN dotnet restore "BCGov.VirusScan.Api/BCGov.VirusScan.Api.csproj"
COPY . .
WORKDIR "/src/BCGov.VirusScan.Api"
RUN dotnet build "BCGov.VirusScan.Api.csproj" -c $BUILD_MODE -o /app/build

FROM build AS publish
ARG BUILD_MODE
RUN dotnet publish "BCGov.VirusScan.Api.csproj" -c $BUILD_MODE -o /app/publish /p:UseAppHost=false
COPY entrypoint.sh /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chgrp 0 entrypoint.sh && chmod a+rx,o-rx entrypoint.sh

ENTRYPOINT ["./entrypoint.sh", "dotnet", "BCGov.VirusScan.Api.dll"]
