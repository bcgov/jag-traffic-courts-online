#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk
ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet

FROM $RUNTIME_IMAGE:7.0 AS base
WORKDIR /app
EXPOSE 8080

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

FROM $BUILD_IMAGE:7.0 AS build
WORKDIR /src
COPY ["TrafficCourts.Arc.Dispute.Service/TrafficCourts.Arc.Dispute.Service.csproj", "TrafficCourts.Arc.Dispute.Service/"]
RUN dotnet restore "TrafficCourts.Arc.Dispute.Service/TrafficCourts.Arc.Dispute.Service.csproj"
COPY . .
WORKDIR "/src/TrafficCourts.Arc.Dispute.Service"
RUN dotnet build "TrafficCourts.Arc.Dispute.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrafficCourts.Arc.Dispute.Service.csproj" -c Release -o /app/publish
COPY entrypoint.sh /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chgrp 0 entrypoint.sh && chmod a+rwx,o-rwx entrypoint.sh
ENTRYPOINT ["./entrypoint.sh", "dotnet", "TrafficCourts.Arc.Dispute.Service.dll"]
