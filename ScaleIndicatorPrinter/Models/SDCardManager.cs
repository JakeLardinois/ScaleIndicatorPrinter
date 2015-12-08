using System;
using Microsoft.SPOT;

using Rinsen.WebServer.FileAndDirectoryServer;


namespace ScaleIndicatorPrinter.Models
{
    public class SDCardManager: SDCard.SDCard, ISDCard
    {
        public SDCardManager(string WorkingDirectory)
            : base(WorkingDirectory)
        {
            
        }
    }
}
