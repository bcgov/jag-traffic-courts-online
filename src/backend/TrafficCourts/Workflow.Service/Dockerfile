#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Workflow.Service/TrafficCourts.Workflow.Service.csproj", "Workflow.Service/"]
COPY ["Common/TrafficCourts.Common.csproj", "Common/"]
COPY ["Messaging/TrafficCourts.Messaging.csproj", "Messaging/"]
RUN dotnet restore "Workflow.Service/TrafficCourts.Workflow.Service.csproj"
COPY . .
WORKDIR "/src/Workflow.Service"
RUN dotnet build "TrafficCourts.Workflow.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrafficCourts.Workflow.Service.csproj" -c Release -o /app/publish
COPY entrypoint.sh /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
RUN chgrp 0 entrypoint.sh && chmod a+rwx,o-rwx entrypoint.sh
# Run container by default as user with id 1001 (default)
USER 1001
ENTRYPOINT ["./entrypoint.sh", "dotnet", "TrafficCourts.Workflow.Service.dll"]