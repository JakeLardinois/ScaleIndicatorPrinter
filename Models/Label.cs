using System;
using Microsoft.SPOT;

using NetMf.CommonExtensions;

namespace ScaleIndicatorPrinter.Models
{
    public class Label
    {
        public static string LabelFormat { get; set; }

        private string mLabelText { get; set; }
        public string LabelText { get { return mLabelText; } }


        public Label(string[] strParams)
        {
            mLabelText = LabelFormat;

            for (int intCounter = strParams.Length - 1; intCounter > -1; intCounter--)
                mLabelText = mLabelText.Replace("~p" + intCounter, strParams[intCounter]);
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
