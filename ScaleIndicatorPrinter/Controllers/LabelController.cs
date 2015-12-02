using System;
using Microsoft.SPOT;

using Rinsen.WebServer;
using ScaleIndicatorPrinter.Models;


namespace ScaleIndicatorPrinter.Controllers
{
    class LabelController : Controller
    {
        public void Update()
        {
            var formCollection = GetFormCollection();

            if (formCollection.ContainsKey("LabelFormat"))
                SetJsonResult(new JsonResult { Success = true, Message = "The Label information has been successfully saved!" });
            else
                SetJsonResult(new JsonResult { Success = false, Message = "Failed to save the Label Info..." });
        }
    }
}
