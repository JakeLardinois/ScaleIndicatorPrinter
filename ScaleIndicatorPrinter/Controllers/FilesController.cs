using System;
using Microsoft.SPOT;

using Rinsen.WebServer;
using ScaleIndicatorPrinter.Models;
using Rinsen.WebServer.FileAndDirectoryServer;


namespace ScaleIndicatorPrinter.Controllers
{
    class FilesController : FileController
    {
        public void Index()
        {
            this.SDCardManager = new SDCardManager(Program.WORKINGDIRECTORY);
            string strHTML = string.Empty;


            try
            {
                strHTML = SDCardManager.ReadTextFile(SDCardManager.GetWorkingDirectoryPath() + "filemanager.html");
                strHTML = strHTML.Substring(1, strHTML.Length - 2); //If I don't remove the first character then the page doesn't get rendered as html...
            }
            catch (Exception objEx)
            {
                Debug.Print("Exception caught in FilesController:\r\n" + objEx.Message);
            }

            SetHtmlResult(strHTML);
        }

        public void Upload()
        {
            this.SDCardManager = new SDCardManager(Program.WORKINGDIRECTORY);
            Debug.Print("got it");
            if (HttpContext.Request.Method == "POST")
            {
                Debug.Print("phase II");
                var doFileUpload = RecieveFile();
                SetJsonResult(new Result { Success = true, Message = doFileUpload }); ;
            }
        }

        
    }

    
}
