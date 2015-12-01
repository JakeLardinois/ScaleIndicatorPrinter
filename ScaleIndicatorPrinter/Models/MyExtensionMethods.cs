using System;
using Microsoft.SPOT;

using NetMf.CommonExtensions;
using Json.NETMF;
using NetduinoRGBLCDShield;


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

        public static string GetColorName(this BacklightColor source)
        {
            switch (source)
            {
                case BacklightColor.Off:
                    return "Off";
                case BacklightColor.Red:
                    return "Red";
                case BacklightColor.Yellow:
                    return "Yellow";
                case BacklightColor.Green:
                    return "Green";
                case BacklightColor.Teal:
                    return "Teal";
                case BacklightColor.Blue:
                    return "Blue";
                case BacklightColor.Violet:
                    return "Violet";
                case BacklightColor.White:
                    return "White";
                default:
                    return "Undefined";
            }
        }

        public static BacklightColor GetBackLightColor(this string source)
        {
            switch (source)
            {
                case "Off":
                    return BacklightColor.Off;
                case "Red":
                    return BacklightColor.Red;
                case "Yellow":
                    return BacklightColor.Yellow;
                case "Green":
                    return BacklightColor.Green;
                case "Teal":
                    return BacklightColor.Teal;
                case "Blue":
                    return BacklightColor.Blue;
                case "Violet":
                    return BacklightColor.Violet;
                case "White":
                    return BacklightColor.White;
                default:
                    return BacklightColor.Off;
            }
        }

        public static string EscapeSingleQuotes(this string source)
        {
            return source.Replace("\'", @"''");
        }
        public static string EscapeDoubleQuotes(this string source)
        {
            return source.Replace("\"", @"""");
        }
        public static string EscapeNewLineCarriageReturn(this string source)
        {
            return source.Replace("\r\n", "\\r\\n");
        }
    }

}
