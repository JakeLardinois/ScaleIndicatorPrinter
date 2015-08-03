using System;
using Microsoft.SPOT;

using System.Text;


namespace ScaleIndicatorPrinter
{
    public class IndicatorData
    {
        static string DATEINDICATOR {get {return "DATE:";}}


        public DateTime DateAndTime { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public double NetWeight { get; set; }
        public double TareWeight { get; set; }
        public double GrossWeight { get; set; }


        public IndicatorData(string strData)
        {
            DateAndTime = DateTime.Now;
            Date = DateTime.Now;
            Time  = DateTime.Now;
            NetWeight = 0.0;
            TareWeight = 0.0;
            GrossWeight = 0.0;

            StringBuilder objStrBldr = new StringBuilder();
            var objParameters = strData.Split(new char[] { '\r' });

            foreach (var strParameter in objParameters)
            {
                objStrBldr.Clear();
                if (strParameter.ToUpper().IndexOf(DATEINDICATOR) != -1)
                {
                    objStrBldr.Append(strParameter.ToUpper()).Replace(DATEINDICATOR, string.Empty);
                    var blah = "dood";
                }
                    
            }
        }
    }
}
