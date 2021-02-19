FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src/
COPY . .
RUN dotnet restore "DisputeApi.Web/DisputeApi.Web.csproj"

RUN dotnet build "DisputeApi.Web/DisputeApi.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DisputeApi.Web/DisputeApi.Web.csproj" -c Release -o /app

FROM base AS final
COPY --from=publish /app .

ENTRYPOINT ["sh","-c","dotnet DisputeApi.Web.dll"]
