using System;
using System.IO;
using System.Text;
using DinkToPdf.EventDefinitions;

namespace DinkToPdf.TestConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Console.ReadLine();
            var converter = new BasicConverter(new PdfTools());

            EventHandler<PhaseChangedArgs> phaseChanged = Converter_PhaseChanged;
            EventHandler<ProgressChangedArgs> progressChanged = Converter_ProgressChanged;
            EventHandler<FinishedArgs> finished = Converter_Finished;
            EventHandler<WarningArgs> warning = Converter_Warning;
            EventHandler<ErrorArgs> error = Converter_Error;

            converter.PhaseChanged += phaseChanged;
            converter.ProgressChanged += progressChanged;
            converter.Finished += finished;
            converter.Warning += warning;
            converter.Error += error;

            foreach (var file in Directory.EnumerateFiles("Input"))
            {
                var doc = new HtmlToPdfDocument
                {
                    GlobalSettings =
                    {
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                    }
                };

                doc.Objects.Add(new ObjectSettings
                {
                    WebSettings =
                    {
                        Background = true, DefaultEncoding = "utf8", LoadImages = true
                    },
                    HtmlContent = File.ReadAllText(file, Encoding.UTF8)
                });

                var pdf = converter.Convert(doc);

                if (!Directory.Exists("Files"))
                {
                    Directory.CreateDirectory("Files");
                }

                using (var stream =
                    new FileStream(
                        Path.Combine("Files", $"{Path.GetFileNameWithoutExtension(file)}.{DateTime.UtcNow.Ticks}.pdf"),
                        FileMode.Create))
                {
                    stream.Write(pdf, 0, pdf.Length);
                }
            }

            converter.PhaseChanged -= phaseChanged;
            converter.ProgressChanged -= progressChanged;
            converter.Finished -= finished;
            converter.Warning -= warning;
            converter.Error -= error;
        }

        private static void Converter_Error(object sender, ErrorArgs e)
        {
            Console.WriteLine("[ERROR] {0}", e.Message);
        }

        private static void Converter_Warning(object sender, WarningArgs e)
        {
            Console.WriteLine("[WARN] {0}", e.Message);
        }

        private static void Converter_Finished(object sender, FinishedArgs e)
        {
            Console.WriteLine("Conversion {0} ", e.Success ? "successful" : "unsuccessful");
        }

        private static void Converter_ProgressChanged(object sender, ProgressChangedArgs e)
        {
            Console.WriteLine("Progress changed {0}", e.Description);
        }

        private static void Converter_PhaseChanged(object sender, PhaseChangedArgs e)
        {
            Console.WriteLine("Phase changed {0} - {1}", e.CurrentPhase, e.Description);
        }
    }
}