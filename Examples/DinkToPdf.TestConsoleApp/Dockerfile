FROM microsoft/dotnet:2.2-runtime-bionic AS base
WORKDIR /app

RUN apt update && apt install -y libgdiplus && rm -rf /var/lib/apt/lists/*


FROM microsoft/dotnet:2.2-sdk-bionic AS build
WORKDIR /src
COPY DinkToPdf.TestConsoleApp/DinkToPdf.TestConsoleApp.csproj DinkToPdf.TestConsoleApp/
COPY . .
RUN dotnet restore DinkToPdf.TestConsoleApp/DinkToPdf.TestConsoleApp.csproj
WORKDIR /src/DinkToPdf.TestConsoleApp
RUN dotnet build DinkToPdf.TestConsoleApp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish DinkToPdf.TestConsoleApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DinkToPdf.TestConsoleApp.dll"]
