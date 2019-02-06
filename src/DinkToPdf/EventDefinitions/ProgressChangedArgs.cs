using System;
using DinkToPdf.Contracts;

namespace DinkToPdf.EventDefinitions
{
    public class ProgressChangedArgs : EventArgs
    {
        public IDocument Document { get; set; }

        public string Description { get; set; }
    }
}