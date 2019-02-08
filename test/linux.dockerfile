FROM microsoft/dotnet:sdk-bionic as base

WORKDIR /src

RUN apt update && apt install -y libgdiplus && rm -rf /var/lib/apt/lists/*

COPY test/* ./
COPY NuGet.config NuGet.config
COPY packages-local packages-local

#ENV LD_DEBUG=libs

CMD dotnet test -f netcoreapp2.2