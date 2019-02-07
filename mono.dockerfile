FROM microsoft/dotnet:2.2-sdk-bionic

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF  && \
    echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | tee /etc/apt/sources.list.d/mono-official-stable.list && \
    apt update && \
    apt-get install -y mono-devel && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /src

COPY src/DinkToPdf src/DinkToPdf
COPY v0.12.5 v0.12.5

RUN msbuild src/DinkToPdf/DinkToPdf.csproj /t:Restore && msbuild src/DinkToPdf/DinkToPdf.csproj /t:Rebuild /p:Configuration=Release
COPY NuGet.config NuGet.config

VOLUME /src/packages-local

CMD dotnet nuget push $(ls src/DinkToPdf/bin/Release/DinkToPdf.?.?.?.nupkg) -s Local