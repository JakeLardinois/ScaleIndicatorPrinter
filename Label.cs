using System;
using Microsoft.SPOT;

using System.Text;


namespace ScaleIndicatorPrinter
{
    public class Label
    {
        private static string CARRIAGERETURN = "\r\n";
        private string LabelFormat = "N" + CARRIAGERETURN +
                                    "A50,100,0,5,1,1,N,\"~p0\"" + CARRIAGERETURN +
                                    "B50,150,0,3,3,7,200,B,\"~p1\"" + CARRIAGERETURN +
                                    "P1" + CARRIAGERETURN;

        public string LabelText { get; set; }

        public Label(string[] strParams)
        {
            StringBuilder objStrBldr = new StringBuilder(LabelFormat);
            //objStrBldr.Replace("~p0", "do0");
            //objStrBldr.Replace("~p1", "do1");

            for (int intCounter = 0; intCounter <= strParams.Length - 1; ++intCounter)
            {
                string strParameter = "~p" + intCounter;

                var strTemp = objStrBldr.ToString();
                objStrBldr.Clear();
                objStrBldr.Append(strTemp.Substring(0, strTemp.IndexOf(strParameter)) + 
                    strParams[intCounter] +
                    strTemp.Substring(strTemp.IndexOf(strParameter) + strParameter.Length));
            }
            LabelText = objStrBldr.ToString();
        }
    }
}
