using System;
using Microsoft.SPOT;

using Rinsen.WebServer;
using ScaleIndicatorPrinter.Models;




namespace ScaleIndicatorPrinter.Controllers
{
    class FilesController : Controller
    {
        public void Index()
        {

        }

        public void Upload()
        {
            Debug.Print("got it");
            if (HttpContext.Request.Method == "POST")
            {
                Debug.Print("phase II");
                SetFileResult();
                //if (HttpContext.Request.Headers["Content-Type"] == "multipart/form-data")
                //{
                //    Debug.Print("Got some form data!");
                //    SetFileResult();
                //}
                    
                
            }
        }

        
    }

    
}
