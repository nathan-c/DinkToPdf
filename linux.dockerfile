FROM microsoft/dotnet:sdk-bionic as base

WORKDIR /src

RUN apt update && apt install -y libgdiplus && rm -rf /var/lib/apt/lists/*

COPY ./src/DinkToPdf ./src/DinkToPdf
COPY ./v0.12.5 ./v0.12.5

RUN dotnet build src/DinkToPdf/DinkToPdf.csproj -c Release



FROM microsoft/dotnet:sdk-bionic

WORKDIR /src

COPY --from=base /src/src/DinkToPdf/bin/Release/DinkToPdf.*.nupkg ./
COPY NuGet.config NuGet.config

VOLUME /src/packages-local

CMD dotnet nuget push $(ls DinkToPdf.?.?.?.nupkg) -s Local