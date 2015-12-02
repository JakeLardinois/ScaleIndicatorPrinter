using System;
using Microsoft.SPOT;

namespace ScaleIndicatorPrinter.Models
{
    class CartScaleWebVariables
    {
        public bool IsDhcpEnabled { get; set; }
        public string NetworkInterfaceType { get; set; }
        public string IPAddress { get; set; }
        public string NetMask { get; set; }
        public string Gateway { get; set; }
        public string MACAddress { get; set; }
        public string DnsAddresses { get; set; }

        public string LabelFormat { get; set; }

        public string Job { get; set; }
        public string Suffix { get; set; }
        public string Operation { get; set; }

        public string ShopTrakTransactionsURL { get; set; }
        public string PieceWeight { get; set; }
        public string NetWeightAdjustment { get; set; }
        public string[] BacklightColors { get; set; }
        public string BacklightColor { get; set; }
    }
}
