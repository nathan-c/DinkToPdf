using System;
using DinkToPdf.Contracts;

namespace DinkToPdf.EventDefinitions
{
    public class FinishedArgs : EventArgs
    {
        public IDocument Document { get; set; }

        public bool Success { get; set; }
    }
}