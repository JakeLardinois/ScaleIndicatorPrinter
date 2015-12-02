using System;
using Microsoft.SPOT;

using Rinsen.WebServer;
using System.IO;
using NetMf.CommonExtensions;
using ScaleIndicatorPrinter.Models;


namespace ScaleIndicatorPrinter.Controllers
{
    class DefaultController : Controller
    {
        public void Index()
        {
            string strHTML = string.Empty;


            try
            {
                using (StreamReader objStreamReader = new StreamReader(@"\SD\WWW\index.html"))
                    strHTML = objStreamReader.ReadToEnd();
                strHTML = strHTML.Substring(1, strHTML.Length - 2); //If I don't remove the first character then the page doesn't get rendered as html...
            }
            catch(Exception objEx)
            {
                Debug.Print("Exception caught in DefaultController:\r\n" + objEx.Message);
            }

            //SetHtmlResult("<!DOCTYPE html><html><body><h1>Default " + Program.Settings.MACAddress + " CartScale Page...</h1></body></html>");
            SetHtmlResult(strHTML);
        }

        public void Settings()
        {
            var intColorCount = (int)NetduinoRGBLCDShield.BacklightColor.ColorCount;
            var objColors = new string[intColorCount];


            for (var intCounter = 0; intCounter < intColorCount; intCounter++)
                objColors[intCounter] = ((NetduinoRGBLCDShield.BacklightColor)intCounter).GetColorName();


                //Populate the Settings NIC variables...
                Program.Settings.RetrieveNetworkSettings(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0]);

            var objFormSettings = new CartScaleWebVariables {
                IsDhcpEnabled = Program.Settings.IsDhcpEnabled,
                NetworkInterfaceType = Program.Settings.NetworkInterfaceType.GetNetworkInterfaceTypeName(),
                IPAddress = Program.Settings.IPAddress,
                NetMask = Program.Settings.NetMask,
                Gateway = Program.Settings.Gateway,
                MACAddress = Program.Settings.MACAddress,
                DnsAddresses = Program.Settings.DnsAddresses.Flatten(),

                LabelFormat = Program.Settings.LabelFormat.EscapeNewLineCarriageReturn(), 
                
                Job = Program.Settings.Job,
                Suffix = Program.Settings.Suffix.ToString("D3"),
                Operation = Program.Settings.Operation,

                ShopTrakTransactionsURL = Program.Settings.ShopTrakTransactionsURL,
                PieceWeight = Program.Settings.PieceWeight.ToString("F3"),
                NetWeightAdjustment = Program.Settings.NetWeightAdjustment.ToString("F3"),
                BacklightColors = objColors,
                BacklightColor = Program.Settings.BacklightColorName
            };

            SetJsonResult(objFormSettings);
        }
    }

    class JSONResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
