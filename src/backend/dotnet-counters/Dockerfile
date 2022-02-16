FROM mcr.microsoft.com/dotnet/sdk:6.0 as tools
RUN dotnet tool install --tool-path /tools dotnet-counters

FROM mcr.microsoft.com/dotnet/runtime:6.0  AS runtime
WORKDIR /tools 
COPY --from=tools /tools .
