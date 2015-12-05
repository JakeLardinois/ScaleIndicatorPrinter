using System;
using Microsoft.SPOT;

using Rinsen.WebServer;


namespace ScaleIndicatorPrinter.Controllers
{
    class FilesController : Controller
    {
        public void Index()
        {
            var SDCard = new SDCard.SDCard();
            string strHTML = string.Empty;


            try
            {
                strHTML = SDCard.ReadTextFile(@"\SD\WWW\filemanager.html");
                strHTML = strHTML.Substring(1, strHTML.Length - 2); //If I don't remove the first character then the page doesn't get rendered as html...
            }
            catch (Exception objEx)
            {
                Debug.Print("Exception caught in FilesController:\r\n" + objEx.Message);
            }

            //SetHtmlResult("<!DOCTYPE html><html><body><h1>Default " + Program.Settings.MACAddress + " CartScale Page...</h1></body></html>");
            SetHtmlResult(strHTML);
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
