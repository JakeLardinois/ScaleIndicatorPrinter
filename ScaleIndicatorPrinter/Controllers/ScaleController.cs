using System;
using Microsoft.SPOT;

using Rinsen.WebServer;
using ScaleIndicatorPrinter.Models;


namespace ScaleIndicatorPrinter.Controllers
{
    class ScaleController : Controller
    {
        public void Index()
        {
            var SDCard = new NetduinoSDCard.SDCard();
            string strHTML = string.Empty;


            try
            {
                strHTML = SDCard.ReadTextFile(@"\SD\WWW\index.html");
                strHTML = strHTML.Substring(1, strHTML.Length - 2); //If I don't remove the first character then the page doesn't get rendered as html...
            }
            catch (Exception objEx)
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
                objColors[intCounter] = ((NetduinoRGBLCDShield.BacklightColor)intCounter).GetName();


            //Populate the Settings NIC variables...
            Program.Settings.RetrieveNetworkSettings(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0]);

            var objFormSettings = new CartScaleWebVariables
            {
                IsDhcpEnabled = Program.Settings.IsDhcpEnabled,
                NetworkInterfaceType = Program.Settings.NetworkInterfaceType.GetName(),
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
                BacklightColor = Program.Settings.BacklightColorName,

                Item = Program.Settings.Item,
                Employees = Program.Settings.Employees,
                PieceCount = Program.Settings.PieceCount.ToString("N"),
                NetWeight = Program.Settings.NetWeight.ToString("F3"),
                PrintDateTime = Program.Settings.PrintDateTime.ToString("MM/dd/yy h:mm:ss tt")
            };

            SetJsonResult(objFormSettings);
        }

        public void Update()
        {
            var formCollection = GetFormCollection();


            if (formCollection.ContainsKey("BacklightColor") && formCollection.ContainsKey("NetWeightAdjustment") && formCollection.ContainsKey("PieceWeight") && formCollection.ContainsKey("ShopTrakTransactionsURL"))
                SetJsonResult(new Result { Success = true, Message = "The Results have been successfully saved!" });
            else
                SetJsonResult(new Result { Success = false, Message = "Failed to save the results..." });
        }
    }
}
