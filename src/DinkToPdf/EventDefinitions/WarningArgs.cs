using System;
using DinkToPdf.Contracts;

namespace DinkToPdf.EventDefinitions
{
    public class WarningArgs : EventArgs
    {
        public IDocument Document { get; set; }

        public string Message { get; set; }
    }
}