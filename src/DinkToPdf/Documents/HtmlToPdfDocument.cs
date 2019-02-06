using System.Collections.Generic;
using DinkToPdf.Contracts;

namespace DinkToPdf
{
    public class HtmlToPdfDocument : IDocument
    {
        public HtmlToPdfDocument()
        {
            Objects = new List<ObjectSettings>();
        }

        public List<ObjectSettings> Objects { get; }

        public GlobalSettings GlobalSettings { get; set; } = new GlobalSettings();

        public IEnumerable<IObject> GetObjects()
        {
            return Objects.ToArray();
        }
    }
}