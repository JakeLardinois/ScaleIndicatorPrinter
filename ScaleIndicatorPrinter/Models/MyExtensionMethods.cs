using System;
using Microsoft.SPOT;

using NetMf.CommonExtensions;


namespace ScaleIndicatorPrinter.Models
{
    public static class MyExtensionMethods
    {
        public static string SetParameters(this string source, string[] Params)
        {
            var strTemp = source;

            for (int intCounter = Params.Length - 1; intCounter > -1; intCounter--)
                strTemp = strTemp.Replace("~p" + intCounter, Params[intCounter]);

            return strTemp;
        }
    }
}
