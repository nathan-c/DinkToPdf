using System;
using System.IO;
using System.Threading.Tasks;

namespace DinkToPdf.TestThreadSafe
{
    public class Program
    {
        private static SynchronizedConverter converter;

        public static void Main(string[] args)
        {
            converter = new SynchronizedConverter(new PdfTools());

            var doc = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Grayscale,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings {Top = 10}
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        Page = "http://www.color-hex.com/"
                    }
                }
            };

            var t1 = Task.Run(() => Action(doc));

            var doc2 = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    PaperSize = PaperKind.A4Small
                },

                Objects =
                {
                    new ObjectSettings
                    {
                        Page = "http://google.com/"
                    }
                }
            };


            var t2 = Task.Run(() => Action(doc2));

            Task.WaitAll(t1, t2);
        }

        private static void Action(HtmlToPdfDocument doc)
        {
            var pdf = converter.Convert(doc);

            if (!Directory.Exists("Files"))
            {
                Directory.CreateDirectory("Files");
            }

            using (var stream = new FileStream(@"Files\" + DateTime.UtcNow.Ticks + ".pdf", FileMode.Create))
            {
                stream.Write(pdf, 0, pdf.Length);
            }
        }
    }
}