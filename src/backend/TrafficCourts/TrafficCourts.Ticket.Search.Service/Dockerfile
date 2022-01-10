#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.access.redhat.com/ubi8/dotnet-60-runtime:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM registry.access.redhat.com/ubi8/dotnet-60:6.0 AS build
WORKDIR /src
COPY ["TrafficCourts.Ticket.Search.Service/TrafficCourts.Ticket.Search.Service.csproj", "TrafficCourts.Ticket.Search.Service/"]
RUN dotnet restore "TrafficCourts.Ticket.Search.Service/TrafficCourts.Ticket.Search.Service.csproj"
COPY . .
WORKDIR "/src/TrafficCourts.Ticket.Search.Service"
RUN dotnet build "TrafficCourts.Ticket.Search.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrafficCourts.Ticket.Search.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrafficCourts.Ticket.Search.Service.dll"]