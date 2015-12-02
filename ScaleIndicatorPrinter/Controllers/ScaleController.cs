using System;
using Microsoft.SPOT;

using Rinsen.WebServer;
using ScaleIndicatorPrinter.Models;


namespace ScaleIndicatorPrinter.Controllers
{
    class ScaleController : Controller
    {
        public void Update()
        {
            var formCollection = GetFormCollection();

            if (formCollection.ContainsKey("BacklightColor") && formCollection.ContainsKey("NetWeightAdjustment") && formCollection.ContainsKey("PieceWeight") && formCollection.ContainsKey("ShopTrakTransactionsURL"))
                SetJsonResult(new JsonResult { Success = true, Message = "The Results have been successfully saved!" });
            else
                SetJsonResult(new JsonResult { Success = false, Message = "Failed to save the results..." });
        }
    }
}
