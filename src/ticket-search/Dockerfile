
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build


WORKDIR /src
COPY . .
RUN dotnet publish "ticket-search/ticket-search.csproj" -c Release -o /opt/app-root

FROM registry.access.redhat.com/ubi8/dotnet-50-runtime AS base
WORKDIR /opt/app-root
EXPOSE 8080
COPY --from=build /opt/app-root .

ENTRYPOINT ["sh","-c","dotnet Gov.TicketSearch.dll"]
