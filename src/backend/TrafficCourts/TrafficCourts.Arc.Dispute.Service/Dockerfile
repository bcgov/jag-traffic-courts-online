#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
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
