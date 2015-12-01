using System;
using Microsoft.SPOT;

using Rinsen.WebServer;
using ScaleIndicatorPrinter.Models;


namespace ScaleIndicatorPrinter.Controllers
{
    class LabelController : Controller
    {
        public void Index()
        {
            var objLabel = new Label { Format = @Program.Settings.LabelFormat.EscapeNewLineCarriageReturn() };
            SetJsonResult(objLabel);
        }

        public void Update()
        {
            var formCollection = GetFormCollection();

            if (formCollection.ContainsKey("LabelFormat"))
                SetJsonResult(new JSONResult { Success = true, Message = "The Results have been successfully saved!" });
            else
                SetJsonResult(new JSONResult { Success = false, Message = "Failed to save the results..." });
        }
    }

    class JSONResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    class Label
    {
        public string Format { get; set; }
    }
}
