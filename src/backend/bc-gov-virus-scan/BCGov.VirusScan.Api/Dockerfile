#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BCGov.VirusScan.Api/BCGov.VirusScan.Api.csproj", "BCGov.VirusScan.Api/"]
RUN dotnet restore "BCGov.VirusScan.Api/BCGov.VirusScan.Api.csproj"
COPY . .
WORKDIR "/src/BCGov.VirusScan.Api"
RUN dotnet build "BCGov.VirusScan.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BCGov.VirusScan.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false
COPY entrypoint.sh /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chgrp 0 entrypoint.sh && chmod a+rx,o-rx entrypoint.sh

ENTRYPOINT ["./entrypoint.sh", "dotnet", "BCGov.VirusScan.Api.dll"]
