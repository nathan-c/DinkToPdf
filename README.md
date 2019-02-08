# DinkToPdf
.NET Core P/Invoke wrapper for wkhtmltopdf library that uses Webkit engine to convert HTML pages to PDF. It is .NET Standard 2.0 and includes wkhtmltopdf binaries for x86/x64 platforms on Windows/Linux/macOS.

### Install 

Library can be installed through Nuget. The NuGet package includes native wkhtmltopdf binaries. Run command bellow from the package manager console:

```
PM> Install-Package DinkToPdf
```

### IMPORTANT
Library was NOT tested with IIS. Library was tested in console applications and with Kestrel web server both for Web Application and Web API . 

### 

### Basic converter
Use this converter in single threaded applications.

Create converter:
```csharp
var converter = new BasicConverter(new PdfTools());
```

### Synchronized converter
Use this converter in multi threaded applications and web servers. Conversion tasks are saved to blocking collection and executed on a single thread.

```csharp
var converter = new SynchronizedConverter(new PdfTools());
```

### Define document to convert
```csharp
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Landscape,
        PaperSize = PaperKind.A4Plus,
    },
    Objects = {
        new ObjectSettings() {
            PagesCount = true,
            HtmlContent = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. In consectetur mauris eget ultrices  iaculis. Ut                               odio viverra, molestie lectus nec, venenatis turpis.",
            WebSettings = { DefaultEncoding = "utf-8" },
            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
        }
    }
};

```

### Convert
If Out property is empty string (defined in GlobalSettings) result is saved in byte array. 
```csharp
byte[] pdf = converter.Convert(doc);
```

If Out property is defined in document then file is saved to disk:
```csharp
var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Portrait,
        PaperSize = PaperKind.A4,
        Margins = new MarginSettings() { Top = 10 },
        Out = @"C:\DinkToPdf\src\DinkToPdf.TestThreadSafe\test.pdf",
    },
    Objects = {
        new ObjectSettings()
        {
            Page = "http://google.com/",
        },
    }
};
```
```csharp
converter.Convert(doc);
```

### Dependency injection
Converter must be registered as singleton.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add converter to DI
    services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
}
```

### Docker support
On Linux the libgdiplus library must be installed in your container. There is an example Dockerfile in the DinkToPdf.TestConsoleApp project.

### Building
Build and run Dockerfile in root container to produce a NuGet package in packages-local
```
docker build -t dinktopdfbuild -f windows.dockerfile .
docker run --rm -v E:/git/DinkToPdf/nathan-c/packages-local:C:/src/packages-local dinktopdfbuild
```

### Updating wkhtmltopdf binaries
Pull the latest binaries from https://wkhtmltopdf.org/downloads.html. On windows take the MXE toolchain bins. The MSVC ones were giving me trouble at runtime. As of v.0.12.5 the linux binaries are not portable. Currently we only pull Ubuntu bins and put them in a correctly labelled runtime directory. In future we may want to create multiple runtime nupkg's, one for each runtime and let user decide what to import.
