#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk
ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet

FROM $RUNTIME_IMAGE:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM $BUILD_IMAGE:7.0 AS build
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
COPY entrypoint.sh /app/publish

FROM base AS final

# Optional Git Version arguments
ARG GIT_COMMIT_DATE=not-set
ARG GIT_SHA=not-set
ARG GIT_SHORT_SHA=not-set
ARG GIT_VERSION=not-set

LABEL git.commit.date="$GIT_COMMIT_DATE" \
      git.sha="$GIT_SHA" \
      git.sha.short="$GIT_SHORT_SHA" \
      git.version="$GIT_VERSION"

ENV APP_VERSION=$GIT_VERSION
ENV GIT_SHA=$GIT_SHA

WORKDIR /app
COPY --from=publish /app/publish .
RUN chgrp 0 entrypoint.sh && chmod a+rx,o-rx entrypoint.sh
ENTRYPOINT ["./entrypoint.sh", "dotnet", "TrafficCourts.Citizen.Service.dll"]