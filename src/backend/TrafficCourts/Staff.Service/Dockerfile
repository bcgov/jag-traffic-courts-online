#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TrafficCourts.Staff.Service/TrafficCourts.Staff.Service.csproj", "TrafficCourts.Staff.Service/"]
RUN dotnet restore "TrafficCourts.Staff.Service/TrafficCourts.Staff.Service.csproj"
COPY . .
WORKDIR "/src/TrafficCourts.Staff.Service"
RUN dotnet build "TrafficCourts.Staff.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrafficCourts.Staff.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrafficCourts.Staff.Service.dll"]
