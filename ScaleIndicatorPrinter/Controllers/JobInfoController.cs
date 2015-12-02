using System;
using Microsoft.SPOT;

using Rinsen.WebServer;
using ScaleIndicatorPrinter.Models;


namespace ScaleIndicatorPrinter.Controllers
{
    class JobInfoController : Controller
    {
        public void Update()
        {
            var formCollection = GetFormCollection();

            if (formCollection.ContainsKey("Job") && formCollection.ContainsKey("Suffix") && formCollection.ContainsKey("Operation"))
                SetJsonResult(new JsonResult { Success = true, Message = "The Job Info has been successfully saved!" });
            else
                SetJsonResult(new JsonResult { Success = false, Message = "Failed to save the Job Info..." });
        }
    }
}
