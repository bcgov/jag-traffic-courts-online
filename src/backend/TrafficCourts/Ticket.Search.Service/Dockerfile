#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ticket.Search.Service/TrafficCourts.Ticket.Search.Service.csproj", "Ticket.Search.Service/"]
COPY ["Common/TrafficCourts.Common.csproj", "Common/"]
RUN dotnet restore "Ticket.Search.Service/TrafficCourts.Ticket.Search.Service.csproj"
COPY . .
WORKDIR "/src/Ticket.Search.Service"
RUN dotnet build "TrafficCourts.Ticket.Search.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrafficCourts.Ticket.Search.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrafficCourts.Ticket.Search.Service.dll"]