using System;
using Microsoft.SPOT;

using System.IO;
using NetMf.CommonExtensions;
using NetduinoRGBLCDShield;
using Microsoft.SPOT.Net.NetworkInformation;
using Rinsen.WebServer.Extensions;


namespace ScaleIndicatorPrinter.Models
{
    public class Settings
    {
        private SDCardManager mSDCard { get; set; }

        public string LabelFormatFileName { get; private set; }
        public string LabelFormat { get; set; }
        public void StoreLabelFormat()
        {
            /*I may have an issue here when the label format saved is smaller than the previous label format. NETMF does not truncate a file when writing contents
             * I tried opening the file with FileMode.Truncate and also tried using the objFileStream.SetLength(0) method. According to the documentation, file truncation is
             * supposed to occur by default when using FileMode.Create...*/
            mSDCard.Write(mSDCard.GetWorkingDirectoryPath(), LabelFormatFileName, FileMode.Create, LabelFormat);
            Debug.Print("Wrote Contents: " + LabelFormat + "\r\nTo File: " + mSDCard.GetWorkingDirectoryPath() + LabelFormatFileName);
        }

        public string JobNumberFileName { get; private set; }
        public string JobNumber { get; set; }
        public void StoreJobNumber()
        {
            mSDCard.WriteLine(mSDCard.GetWorkingDirectoryPath(), JobNumberFileName, FileMode.Create, JobNumber);
            Debug.Print("Wrote Contents: " + Job + "\r\nTo File: " + mSDCard.GetWorkingDirectoryPath() + JobNumberFileName);
        }
        public string Job { 
            get {
                if (JobNumber.IndexOf('-') != -1)
                {
                    var intPlaceholder = JobNumber.LastIndexOf('-');
                    return JobNumber.Substring(0, intPlaceholder);
                }
                else
                    return "B";
            } 
        }
        public int Suffix {
            get
            {
                try
                {
                    var intPlaceholder = JobNumber.LastIndexOf('-');
                    return int.Parse(JobNumber.Substring(intPlaceholder + 1, JobNumber.Length - intPlaceholder - 1));
                }
                catch
                {
                    return int.MaxValue;
                }
            } 
        }

        public string OperationFileName { get; private set; }
        public string Operation { get; set; }
        public void StoreOperationNumber()
        {
            mSDCard.WriteLine(mSDCard.GetWorkingDirectoryPath(), OperationFileName, FileMode.Create, Operation);
            Debug.Print("Wrote Contents: " + Operation + "\r\nTo File: " + mSDCard.GetWorkingDirectoryPath() + OperationFileName);
        }
        public int OperationNumber
        {
            get
            {
                try
                {
                    return int.Parse(Operation);
                }
                catch
                {
                    return int.MaxValue;
                }

            }
        }

        public string ShopTrakTransactionsURLFileName { get; private set; }
        public string ShopTrakTransactionsURL { get; set; }
        public void StoreShopTrakTransactionsURL()
        {
            mSDCard.WriteLine(mSDCard.GetWorkingDirectoryPath(), ShopTrakTransactionsURLFileName, FileMode.Create, ShopTrakTransactionsURL);
            Debug.Print("Wrote Contents: " + ShopTrakTransactionsURL + "\r\nTo File: " + mSDCard.GetWorkingDirectoryPath() + ShopTrakTransactionsURLFileName);
        }

        public double[] Increments { get; set; }
        private int mintIncrementSelection { get; set; }
        public int IncrementSelection
        {
            get
            {
                return mintIncrementSelection;
            }
            set
            {
                if (value >= 0 && value <= Increments.Length - 1)
                    mintIncrementSelection = System.Math.Abs(value);
            }
        }

        public string PieceWeightFileName { get; private set; }
        private double mdblPieceWeight { get; set; }
        public double PieceWeight { 
            get { return mdblPieceWeight; } 
            set { mdblPieceWeight = value > 0 ? value : 0; } 
        }
        public void StorePieceWeight()
        {
            mSDCard.WriteLine(mSDCard.GetWorkingDirectoryPath(), PieceWeightFileName, FileMode.Create, PieceWeight.ToString("F3"));
            Debug.Print("Wrote Contents: " + PieceWeight.ToString("F3") + "\r\nTo File: " + mSDCard.GetWorkingDirectoryPath() + PieceWeightFileName);
        }
        public void IncrementPieceWeight()
        {
            PieceWeight = PieceWeight + Increments[IncrementSelection];
            Debug.Print("PieceWeight Incremented to: " + PieceWeight);
        }
        public void DecrementPieceWeight()
        {
            PieceWeight = PieceWeight - Increments[IncrementSelection];
            Debug.Print("PieceWeight Decremented to: " + PieceWeight);
        }

        public string NetWeightAdjustmentFileName { get; private set; }
        public double NetWeightAdjustment{ get; set; }
        public void StoreNetWeightAdjustment()
        {
            mSDCard.WriteLine(mSDCard.GetWorkingDirectoryPath(), NetWeightAdjustmentFileName, FileMode.Create, NetWeightAdjustment.ToString("F3"));
            Debug.Print("Wrote Contents: " + NetWeightAdjustment + "\r\nTo File: " + mSDCard.GetWorkingDirectoryPath() + NetWeightAdjustmentFileName);
        }
        public void IncrementNetWeightAdjustment()
        {
            NetWeightAdjustment = NetWeightAdjustment + Increments[IncrementSelection];
            Debug.Print("NetWeightAdjustment Incremented to: " + NetWeightAdjustment);
        }
        public void DecrementNetWeightAdjustment()
        {
             NetWeightAdjustment = NetWeightAdjustment - Increments[IncrementSelection];
             Debug.Print("NetWeightAdjustment Decremented to: " + NetWeightAdjustment);
        }

        public string BacklightColorFileName { get; private set; }
        private int mintBacklightColor { get; set; }
        public BacklightColor BacklightColor
        {
            get { return (BacklightColor)mintBacklightColor; }
            set { mintBacklightColor = (int)value; }
        }
        public string BacklightColorName { get { return BacklightColor.GetName(); } }
        public void StoreBacklightColor()
        {
            mSDCard.WriteLine(mSDCard.GetWorkingDirectoryPath(), BacklightColorFileName, FileMode.Create, BacklightColor.ToString());
            Debug.Print("Wrote Contents: " + BacklightColor + "\r\nTo File: " + mSDCard.GetWorkingDirectoryPath() + BacklightColorFileName);
        }
        public void NextBacklightColor()
        {
            mintBacklightColor = ++mintBacklightColor % (int)BacklightColor.ColorCount;
        }
        public void PreviousBacklightColor()
        {
            if (mintBacklightColor == 0)
                mintBacklightColor = (int)BacklightColor.ColorCount;
            mintBacklightColor = --mintBacklightColor % (int)BacklightColor.ColorCount;
        }

        public bool IsDhcpEnabled { get; private set; }
        public NetworkInterfaceType NetworkInterfaceType { get; private set; }
        public string MACAddress { get; private set; }
        public string IPAddress { get; private set; }
        public string NetMask { get; private set; }
        public string Gateway { get; private set; }
        public string[] DnsAddresses { get; private set; }
        public void RetrieveNetworkSettings(NetworkInterface objNic)
        {
            //var objNic = Microsoft.SPOT.Net.NetworkInformation.Wireless80211.GetAllNetworkInterfaces()[0];
            //var objNic = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            IsDhcpEnabled = objNic.IsDhcpEnabled;
            NetworkInterfaceType = objNic.NetworkInterfaceType;
            IPAddress = objNic.IPAddress;
            NetMask = objNic.SubnetMask;
            MACAddress = objNic.PhysicalAddress.ToHexString();
            Gateway = objNic.GatewayAddress;
            DnsAddresses = objNic.DnsAddresses;

            Debug.Print("Is DHCP Enabled: " + objNic.IsDhcpEnabled);
            Debug.Print("NIC Type: " + objNic.NetworkInterfaceType.GetName());
            Debug.Print("MAC Address: " + MACAddress);
            Debug.Print("IP Address: " + IPAddress);
            Debug.Print("NetMask: " + NetMask);
            Debug.Print("Gateway: " + objNic.GatewayAddress);
            foreach (var strDnsAddress in DnsAddresses)
                Debug.Print("Dns address: " + strDnsAddress);
        }

        public string Item { get; set; }
        public string Employees { get; set; }
        public int PieceCount { get { return (int)((NetWeight + NetWeightAdjustment) / PieceWeight); } }
        public double NetWeight { get; set; }
        public DateTime PrintDateTime { get; set; }

        public void RetrieveSettingsFromSDCard(string DirectoryPath, string labelformatfilename, string jobnumberfilename, string operationfilename,
            string shoptraktransactionsurlfilename, string pieceweightfilename, string netweightadjustmentfilename, string backlightcolorfilename)
        {
            mSDCard = new SDCardManager(DirectoryPath);

            LabelFormatFileName = labelformatfilename;
            if (File.Exists(mSDCard.GetWorkingDirectoryPath() + LabelFormatFileName))
                RetrieveInformationFromFile(LabelFormatFileName, InformationType.LabelFormat);
            else
                CreateInformationFile(LabelFormatFileName, InformationType.LabelFormat);

            JobNumberFileName = jobnumberfilename;
            if (File.Exists(mSDCard.GetWorkingDirectoryPath() + JobNumberFileName))
                RetrieveInformationFromFile(JobNumberFileName, InformationType.JobNumber);
            else
                CreateInformationFile(JobNumberFileName, InformationType.JobNumber);

            OperationFileName = operationfilename;
            if (File.Exists(mSDCard.GetWorkingDirectoryPath() + OperationFileName))
                RetrieveInformationFromFile(OperationFileName, InformationType.OperationNumber);
            else
                CreateInformationFile(OperationFileName, InformationType.OperationNumber);

            ShopTrakTransactionsURLFileName = shoptraktransactionsurlfilename;
            if (File.Exists(mSDCard.GetWorkingDirectoryPath() + ShopTrakTransactionsURLFileName))
                RetrieveInformationFromFile(ShopTrakTransactionsURLFileName, InformationType.ShopTrakTransactionsURL);
            else
                CreateInformationFile(ShopTrakTransactionsURLFileName, InformationType.ShopTrakTransactionsURL);

            PieceWeightFileName = pieceweightfilename;
            if (File.Exists(mSDCard.GetWorkingDirectoryPath() + PieceWeightFileName))
                RetrieveInformationFromFile(PieceWeightFileName, InformationType.PieceWeight);
            else
                CreateInformationFile(PieceWeightFileName, InformationType.PieceWeight);

            NetWeightAdjustmentFileName = netweightadjustmentfilename;
            if (File.Exists(mSDCard.GetWorkingDirectoryPath() + NetWeightAdjustmentFileName))
                RetrieveInformationFromFile(NetWeightAdjustmentFileName, InformationType.NetWeightAdjustment);
            else
                CreateInformationFile(NetWeightAdjustmentFileName, InformationType.NetWeightAdjustment);

            BacklightColorFileName = backlightcolorfilename;
            if (File.Exists(mSDCard.GetWorkingDirectoryPath() + BacklightColorFileName))
                RetrieveInformationFromFile(BacklightColorFileName, InformationType.ColorName);
            else
                CreateInformationFile(BacklightColorFileName, InformationType.ColorName);
        }

        private void RetrieveInformationFromFile(string FileName, InformationType Information)
        {
            double dblTemp;
            string FilePathAndName = mSDCard.GetWorkingDirectoryPath() + FileName;

            
            Debug.Print("Reading file: " + FilePathAndName);
            try
            {
                switch (Information)
                {
                    case InformationType.LabelFormat:
                        LabelFormat = mSDCard.ReadTextFile(FilePathAndName);
                        Debug.Print("LabelFormat Read from SD Card:\r\n" + LabelFormat);
                        break;
                    case InformationType.JobNumber:
                        JobNumber = mSDCard.ReadLine(FilePathAndName);
                        Debug.Print("JobNumber Read from SD Card:\r\n" + JobNumber);
                        break;
                    case InformationType.OperationNumber:
                        Operation = mSDCard.ReadLine(FilePathAndName);
                        Debug.Print("Operation Read from SD Card:\r\n" + Operation);
                        break;
                    case InformationType.ShopTrakTransactionsURL:
                        ShopTrakTransactionsURL = mSDCard.ReadLine(FilePathAndName);
                        Debug.Print("ShopTrakTransactionsURL Read from SD Card:\r\n" + ShopTrakTransactionsURL);
                        break;
                    case InformationType.PieceWeight:
                        var strPieceWeight = mSDCard.ReadLine(FilePathAndName);
                        PieceWeight = double.TryParse(strPieceWeight, out dblTemp) ? dblTemp : 0.0;
                        Debug.Print("PieceWeight Read from SD Card:\r\n" + PieceWeight);
                        break;
                    case InformationType.NetWeightAdjustment:
                        var strNetWeightAdjustment = mSDCard.ReadLine(FilePathAndName);
                        NetWeightAdjustment = double.TryParse(strNetWeightAdjustment, out dblTemp) ? dblTemp : 0.0;
                        Debug.Print("NetWeightAdjustment Read from SD Card:\r\n" + NetWeightAdjustment);
                        break;
                    case InformationType.ColorName:
                        var strBackgroundColor = mSDCard.ReadLine(FilePathAndName);
                        BacklightColor = double.TryParse(strBackgroundColor, out dblTemp) ? (BacklightColor)dblTemp : (BacklightColor)0;
                        Debug.Print("BackgroundColor Read from SD Card:\r\n" + BacklightColor);
                        break;
                }
            }
            catch (NullReferenceException)
            {
                Debug.Print("Invalid (Null) File Contents.  Deleting File...");
                File.Delete(FilePathAndName);
                Debug.Print("Deleted File: " + FilePathAndName);
                CreateInformationFile(FileName, Information);
            }
        }

        private void CreateInformationFile(string FileName, InformationType Information)
        {
            var FileContents = string.Empty;
            var FilePathAndName = mSDCard.GetWorkingDirectoryPath() + FileName;


            Debug.Print("Creating  " + mSDCard.GetWorkingDirectoryPath() + FileName + "...");
            switch (Information)
            {
                case InformationType.LabelFormat:
                    FileContents = Label.DefaultLabel;
                    LabelFormat = Label.DefaultLabel;
                    break;
                case InformationType.JobNumber:
                    FileContents = "B000000123-000";
                    JobNumber = FileContents;
                    break;
                case InformationType.OperationNumber:
                    FileContents = "10";
                    Operation = FileContents;
                    break;
                case InformationType.ShopTrakTransactionsURL:
                    FileContents = "http://dataservice.wiretechfab.com:6156/SytelineDataService/ShopTrak/LCLTTransaction/Job=~p0&Suffix=~p1&Operation=~p2";
                    ShopTrakTransactionsURL = FileContents;
                    break;
                case InformationType.PieceWeight:
                    var dblPieceWeight = 0.0000001;
                    PieceWeight = dblPieceWeight;
                    FileContents = dblPieceWeight.ToString();
                    break;
                case InformationType.NetWeightAdjustment:
                    var dblNetWeightAdjustment = 0.0;
                    NetWeightAdjustment = dblNetWeightAdjustment;
                    FileContents = dblNetWeightAdjustment.ToString();
                    break;
                case InformationType.ColorName:
                    var enumBacklightColor = BacklightColor.White;
                    BacklightColor = enumBacklightColor;
                    FileContents = enumBacklightColor.ToString();
                    break;
            }
            mSDCard.WriteLine(mSDCard.GetWorkingDirectoryPath(), FileName, FileMode.Create, FileContents);
            Debug.Print("Wrote contents: \r\n" + FileContents + "\r\nTo File: " + FilePathAndName);
            Debug.Print("Finished Write...");
        }
    }
}
