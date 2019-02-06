# escape=`

FROM microsoft/dotnet-framework:4.7.2-sdk as base

WORKDIR C:/src

COPY src/DinkToPdf src/DinkToPdf
COPY v0.12.5 v0.12.5

RUN dotnet build src/DinkToPdf/DinkToPdf.csproj -c Release
COPY NuGet.config NuGet.config

VOLUME C:/src/packages-local

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop';"]

CMD dotnet nuget push $(ls src/DinkToPdf/bin/Release/DinkToPdf.?.?.?.nupkg).FullName -s Local