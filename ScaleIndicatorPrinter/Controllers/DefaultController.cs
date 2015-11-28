using System;
using Microsoft.SPOT;
using Rinsen.WebServer;

namespace ScaleIndicatorPrinter.Controllers
{
    class DefaultController : Controller
    {
        public void Index()
        {
            SetHtmlResult("<!DOCTYPE html><html><body><h1>Default CartScal Page...</h1></body></html>");
        }
    }
}
