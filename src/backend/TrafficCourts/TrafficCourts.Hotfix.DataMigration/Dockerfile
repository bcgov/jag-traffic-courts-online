# Use the official .NET SDK image as a base image.
# This image contains the .NET CLI and runtime.
FROM mcr.microsoft.com/dotnet/sdk:8.0


WORKDIR /vsdbg

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    unzip \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL https://aka.ms/getvsdbgsh \
    | bash /dev/stdin -v latest -l /vsdbg


# Set the working directory inside the container.
WORKDIR /app
