using System;
using Microsoft.SPOT;

using Rinsen.WebServer.FileAndDirectoryServer;


namespace ScaleIndicatorPrinter.Models
{
    public class SDCardManager : NetduinoSDCard.SDCard, ISDCardManager
    {
        public SDCardManager(string WorkingDirectory)
            : base(WorkingDirectory)
        {
            
        }
    }
}
