using System;
using Microsoft.SPOT;
using Rinsen.WebServer.Http;

namespace ScaleIndicatorPrinter.Models
{
    public class Forbidden : HttpStatusCode
    {
        public override string ReasonPhrase
        {
            get { return "Forbidden"; }
        }

        public override int StatusCode
        {
            get { return 403; }
        }
    }
}
