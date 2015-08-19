using System;
using Microsoft.SPOT;

using NetMf.CommonExtensions;
using Json.NETMF;


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

        //my extension method that converts the JSON time format of /Date(1376625062603)/ to a DateTime object
        public static DateTime GetDateTimeFromJSON(this string source)
        {
            double dblTemp;

            //return new DateTime(1970, 1, 1)
            //    .AddMilliseconds(
            //    double.TryParse(
            //    source.Replace("/Date(", "").Replace(")/", ""),
            //    out dblTemp) ? dblTemp : 0);
            
            
            var objDateTime = new DateTime(1970, 1, 1);
            var dblstring = source.Replace("/Date(", "").Replace(")/", "");
            var strTemp = dblstring.Substring(0, dblstring.LastIndexOf('-'));
            var milliseconds = double.Parse(strTemp);
            //Microsoft.SPOT.Time.TimeService.UpdateNow(5)
            var blah = DateTimeExtensions.FromASPNetAjax(strTemp);
            return objDateTime.AddMilliseconds(milliseconds);
        }
    }
}
