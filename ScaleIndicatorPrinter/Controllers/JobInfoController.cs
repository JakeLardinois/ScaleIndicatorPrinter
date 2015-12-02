using System;
using Microsoft.SPOT;

using Rinsen.WebServer;


namespace ScaleIndicatorPrinter.Controllers
{
    class JobInfoController : Controller
    {
        public void Update()
        {
            var formCollection = GetFormCollection();

            if (formCollection.ContainsKey("Job") && formCollection.ContainsKey("Suffix") && formCollection.ContainsKey("Operation"))
                SetJsonResult(new JSONResult { Success = true, Message = "The Results have been successfully saved!" });
            else
                SetJsonResult(new JSONResult { Success = false, Message = "Failed to save the results..." });
        }
    }
}
