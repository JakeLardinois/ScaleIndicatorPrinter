using System;
using Microsoft.SPOT;

using System.Text;
using NetMf.CommonExtensions;


namespace ScaleIndicatorPrinter.Models
{
    public class IndicatorData
    {
        private string NetWeightIndicator { get { return "Net"; } }
        private string TareWeightIndicator { get { return "Tare"; } }
        private string GrossWeightIndicator { get { return "Gross"; } }
        private string LBIndicator { get { return "lb"; } }
        private string KGIndicator { get { return "kg"; } }

        public double NetWeight { get; set; }
        public string NetWeightUofM { get; set; }
        public bool HasValidNetWeight { get; set; }
        public double TareWeight { get; set; }
        public string TareWeightUofM { get; set; }
        public bool HasValidTareWeight { get; set; }
        public double GrossWeight { get; set; }
        public string GrossWeightUofM { get; set; }
        public bool HasValidGrossWeight { get; set; }

        public bool HasValidDataString { get { return HasValidNetWeight && HasValidTareWeight && HasValidGrossWeight; } }


        public IndicatorData(string strData)
        {
            var objParameters = strData.Split(new char[] { '\r', '\n' });
            double dblTemp;

            foreach (var strParameter in objParameters)
            {
                if (strParameter == string.Empty || strParameter == null)
                    continue;

                if (strParameter.ToUpper().IndexOf(NetWeightIndicator.ToUpper()) != -1)
                {
                    //NetWeight = Convert.ToDouble(StripTextData(strParameter));
                    HasValidNetWeight = Parse.TryParseDouble(StripTextData(strParameter), out dblTemp);
                    NetWeight = HasValidNetWeight ? dblTemp : 0.0;

                    NetWeightUofM = GetUnitOfMeasure(strParameter);
                    continue;
                }

                if (strParameter.ToUpper().IndexOf(TareWeightIndicator.ToUpper()) != -1)
                {
                    //TareWeight = Convert.ToDouble(StripTextData(strParameter));
                    HasValidTareWeight = Parse.TryParseDouble(StripTextData(strParameter), out dblTemp);
                    TareWeight = HasValidTareWeight ? dblTemp : 0.0;

                    TareWeightUofM = GetUnitOfMeasure(strParameter);
                    continue;
                }

                if (strParameter.ToUpper().IndexOf(GrossWeightIndicator.ToUpper()) != -1)
                {
                    //GrossWeight = Convert.ToDouble(StripTextData(strParameter));
                    HasValidGrossWeight = Parse.TryParseDouble(StripTextData(strParameter), out dblTemp);
                    GrossWeight = HasValidGrossWeight ? dblTemp : 0.0;

                    GrossWeightUofM = GetUnitOfMeasure(strParameter);
                    continue;
                }
            }
        }

        private string StripTextData(string strParameter)
        {
            System.Text.StringBuilder objStrBldr = new System.Text.StringBuilder();

            objStrBldr.Append(strParameter.ToUpper());
            objStrBldr.Replace(NetWeightIndicator.ToUpper(), string.Empty);
            objStrBldr.Replace(TareWeightIndicator.ToUpper(), string.Empty);
            objStrBldr.Replace(GrossWeightIndicator.ToUpper(), string.Empty);
            objStrBldr.Replace(LBIndicator.ToUpper(), string.Empty);
            objStrBldr.Replace(KGIndicator.ToUpper(), string.Empty);
            objStrBldr.Replace(" ", string.Empty);
            return objStrBldr.ToString();
        }

        private string GetUnitOfMeasure(string strParameter)
        {
            if (strParameter.ToUpper().IndexOf(LBIndicator.ToUpper()) != -1)
                return LBIndicator;
            else if (strParameter.ToUpper().IndexOf(KGIndicator.ToUpper()) != -1)
                return KGIndicator;
            else
                return "Unknown";
        }
    }
}
