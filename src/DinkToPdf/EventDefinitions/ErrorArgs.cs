using System;
using DinkToPdf.Contracts;

namespace DinkToPdf.EventDefinitions
{
    public class ErrorArgs : EventArgs
    {
        public IDocument Document { get; set; }

        public string Message { get; set; }
    }
}