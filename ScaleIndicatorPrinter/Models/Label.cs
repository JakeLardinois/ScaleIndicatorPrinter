using System;
using Microsoft.SPOT;


namespace ScaleIndicatorPrinter.Models
{
    public class Label
    {
        public static string LabelFormat { get; set; }
        public static string SampleLabel
        {
            get
            {
                return "N\r\n" +
                    "A50,100,0,5,1,1,N,\"EXAMPLE 1\"\r\n" +
                    "B50,150,0,3,3,7,200,B,\"EXAMPLE 1\"\r\n" +
                    "P1\r\n";
            }
        }

        private string mLabelText { get; set; }
        public string LabelText { get { return mLabelText; } }


        public Label(string[] strParams)
        {
            mLabelText = LabelFormat.SetParameters(strParams);
        }


        //public Label(string[] strParams)
        //{
        //    StringBuilder objStrBldr = new StringBuilder(LabelFormat);
        //    //Found out that the .Net Micro Framework StringBuilder will throw an 'OutOfRangeException' when the .Replace() method is called and the string you are replacing
        //    //is shorter than the string that is replacing it...
        //    //objStrBldr.Replace("~p0", "do0");
        //    //objStrBldr.Replace("~p1", "do1");

        //    for (int intCounter = 0; intCounter <= strParams.Length - 1; ++intCounter)
        //    {
        //        string strParameter = "~p" + intCounter;

        //        var strTemp = objStrBldr.ToString();
        //        objStrBldr.Clear();
        //        objStrBldr.Append(strTemp.Substring(0, strTemp.IndexOf(strParameter)) +
        //            strParams[intCounter] +
        //            strTemp.Substring(strTemp.IndexOf(strParameter) + strParameter.Length));
        //    }
        //    LabelText = objStrBldr.ToString();
        //}
    }
}
