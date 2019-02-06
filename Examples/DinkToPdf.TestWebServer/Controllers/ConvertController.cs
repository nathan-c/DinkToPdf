using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DinkToPdf.TestWebServer.Controllers
{
    [Route("api/[controller]")]
    public class ConvertController : Controller
    {
        private readonly IConverter _converter;

        public ConvertController(IConverter converter, ILogger<ConvertController> logger)
        {
            _converter = converter;

            _converter.PhaseChanged += (o, a) => logger.LogInformation($"Phase Change {a.Description}");
            _converter.ProgressChanged += (o, a) => logger.LogInformation($"Progress Change {a.Description}");
            _converter.Finished += (o, a) => logger.LogInformation($"Finished {a.Document.ToString()}");
            _converter.Warning += (o, a) => logger.LogWarning($"Warn {a.Message}");
            _converter.Error += (o, a) => logger.LogError($"Err {a.Message}");
        }

        // GET api/convert
        [HttpGet]
        public IActionResult Get()
        {
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    PaperSize = PaperKind.A3,
                    Orientation = Orientation.Landscape
                },

                Objects =
                {
                    new ObjectSettings
                    {
                        Page = "http://www.google.com/"
                    }
                }
            };

            var pdf = _converter.Convert(doc);


            return new FileContentResult(pdf, "application/pdf");
        }
    }
}