using System.Collections.Generic;

namespace DinkToPdf.Contracts
{
    public interface IDocument : ISettings
    {
        IEnumerable<IObject> GetObjects();
    }
}