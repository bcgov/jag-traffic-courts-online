ARG DOTNET_SDK_VERSION=5.0
ARG DOTNET_STARTUP_PROJECT=ticket-search/ticket-search.csproj
ARG DOTNET_RUNTIME_BASE=registry.access.redhat.com/ubi8/dotnet-50-runtime

FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_SDK_VERSION AS build

WORKDIR /src
COPY . .
RUN dotnet publish $DOTNET_STARTUP_PROJECT -c Release -o /opt/app-root

FROM $DOTNET_RUNTIME_BASE AS base
WORKDIR /opt/app-root
EXPOSE 8080
COPY --from=build /opt/app-root .

ENV DOTNET_ASSEMBLY_NAME=Gov.TicketSearch.dll
ENTRYPOINT ["sh","-c", "dotnet ${DOTNET_ASSEMBLY_NAME}"]
