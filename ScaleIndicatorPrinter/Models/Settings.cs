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
        private string mRootDirectoryPath { get; set; }
        private DirectoryInfo mRootDirectory { get; set; }

        private string mLabelFormatFileName { get; set; }
        public string LabelFormat { get; set; }
        public void StoreLabelFormat()
        {
            string strFilePathAndName = mRootDirectory.FullName + "\\" + mLabelFormatFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(LabelFormat);
            Debug.Print("Wrote Contents: " + LabelFormat + "\r\nTo File: " + strFilePathAndName);
        }

        private string mJobNumberFileName { get; set; }
        public string JobNumber { get; set; }
        public void StoreJobNumber()
        {
            string strFilePathAndName = mRootDirectory.FullName + "\\" + mJobNumberFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(Job);
            Debug.Print("Wrote Contents: " + Job + "\r\nTo File: " + strFilePathAndName);
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

        private string mOperationFileName { get; set; }
        public string Operation { get; set; }
        public void StoreOperationNumber()
        {
            string strFilePathAndName = mRootDirectory.FullName + "\\" + mOperationFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(Operation);
            Debug.Print("Wrote Contents: " + Operation + "\r\nTo File: " + strFilePathAndName);
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

        private string mShopTrakTransactionsURLFileName { get; set; }
        public string ShopTrakTransactionsURL { get; set; }
        public void StoreShopTrakTransactionsURL()
        {
            string strFilePathAndName = mRootDirectory.FullName + "\\" + mShopTrakTransactionsURLFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(ShopTrakTransactionsURL);
            Debug.Print("Wrote Contents: " + ShopTrakTransactionsURL + "\r\nTo File: " + strFilePathAndName);
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

        private string mPieceWeightFileName { get; set; }
        private double mdblPieceWeight { get; set; }
        public double PieceWeight { 
            get { return mdblPieceWeight; } 
            set { mdblPieceWeight = value > 0 ? value : 0; } 
        }
        public void StorePieceWeight()
        {
            string strFilePathAndName = mRootDirectory.FullName + "\\" + mPieceWeightFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(PieceWeight);
            Debug.Print("Wrote Contents: " + PieceWeight + "\r\nTo File: " + strFilePathAndName);
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

        private string mNetWeightAdjustmentFileName { get; set; }
        public double NetWeightAdjustment{ get; set; }
        public void StoreNetWeightAdjustment()
        {
            string strFilePathAndName = mRootDirectory.FullName + "\\" + mNetWeightAdjustmentFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(NetWeightAdjustment);
            Debug.Print("Wrote Contents: " + NetWeightAdjustment + "\r\nTo File: " + strFilePathAndName);
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

        private string mBacklightColorFileName { get; set; }
        private int mintBacklightColor { get; set; }
        public BacklightColor BacklightColor
        {
            get { return (BacklightColor)mintBacklightColor; }
            set { mintBacklightColor = (int)value; }
        }
        public string BacklightColorName { get { return BacklightColor.GetColorName(); } }
        public void StoreBacklightColor()
        {
            string strFilePathAndName = mRootDirectory.FullName + "\\" + mBacklightColorFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(BacklightColor);
            Debug.Print("Wrote Contents: " + BacklightColor + "\r\nTo File: " + strFilePathAndName);
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

        public bool IsDhcpEnabled { get { return mIsDhcpEnabled; } }
        private bool mIsDhcpEnabled { get; set; }
        public NetworkInterfaceType NetworkInterfaceType { get { return mNetworkInterfaceType; } }
        private NetworkInterfaceType mNetworkInterfaceType { get; set; }
        public string MACAddress { get { return mMACAddress; } }
        private string mMACAddress { get; set; }
        public string IPAddress { get { return mIPAddress; } }
        private string mIPAddress { get; set; }
        public string NetMask { get { return mNetMask; } }
        private string mNetMask { get; set; }
        public string Gateway { get { return mGateway; } }
        private string mGateway { get; set; }
        public string[] DnsAddresses { get { return mDnsAddresses; } }
        private string[] mDnsAddresses { get; set; }
        public void RetrieveNetworkSettings(NetworkInterface objNic)
        {
            //var objNic = Microsoft.SPOT.Net.NetworkInformation.Wireless80211.GetAllNetworkInterfaces()[0];
            //var objNic = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            mIsDhcpEnabled = objNic.IsDhcpEnabled;
            mNetworkInterfaceType = objNic.NetworkInterfaceType;
            mIPAddress = objNic.IPAddress;
            mNetMask = objNic.SubnetMask;
            mMACAddress = objNic.PhysicalAddress.ToHexString();
            mGateway = objNic.GatewayAddress;
            mDnsAddresses = objNic.DnsAddresses;

            Debug.Print("Is DHCP Enabled: " + objNic.IsDhcpEnabled);
            Debug.Print("NIC Type: " + objNic.NetworkInterfaceType.GetNetworkInterfaceTypeName());
            Debug.Print("MAC Address: " + MACAddress);
            Debug.Print("IP Address: " + IPAddress);
            Debug.Print("NetMask: " + NetMask);
            Debug.Print("Gateway: " + objNic.GatewayAddress);
            foreach (var strDnsAddress in DnsAddresses)
                Debug.Print("Dns address: " + strDnsAddress);
        }

        public void RetrieveSettingsFromSDCard(string RootDirectoryPath, string LabelFormatFileName, string JobNumberFileName, string OperationFileName,
            string ShopTrakTransactionsURLFileName, string PieceWeightFileName, string NetWeightAdjustmentFileName, string BackgroundColorFileName)
        {
            mRootDirectoryPath = RootDirectoryPath;
            mRootDirectory = new System.IO.DirectoryInfo(mRootDirectoryPath);
            if (!mRootDirectory.Exists)
            {
                Debug.Print(mRootDirectory.FullName + " is not a Valid Directory!!");
                Debug.Print("Creating " + mRootDirectoryPath + "...");
                Directory.CreateDirectory(mRootDirectoryPath);
            }


            mLabelFormatFileName = LabelFormatFileName;
            if (File.Exists(mRootDirectory.FullName + "\\" + mLabelFormatFileName))
                RetrieveInformationFromFile(mLabelFormatFileName, InformationType.LabelFormat);
            else
                CreateInformationFile(mLabelFormatFileName, InformationType.LabelFormat);

            mJobNumberFileName = JobNumberFileName;
            if (File.Exists(mRootDirectory.FullName + "\\" + mJobNumberFileName))
                RetrieveInformationFromFile(JobNumberFileName, InformationType.JobNumber);
            else
                CreateInformationFile(mJobNumberFileName, InformationType.JobNumber);

            mOperationFileName = OperationFileName;
            if (File.Exists(mRootDirectory.FullName + "\\" + mOperationFileName))
                RetrieveInformationFromFile(mOperationFileName, InformationType.OperationNumber);
            else
                CreateInformationFile(mOperationFileName, InformationType.OperationNumber);

            mShopTrakTransactionsURLFileName = ShopTrakTransactionsURLFileName;
            if (File.Exists(mRootDirectory.FullName + "\\" + mShopTrakTransactionsURLFileName))
                RetrieveInformationFromFile(mShopTrakTransactionsURLFileName, InformationType.ShopTrakTransactionsURL);
            else
                CreateInformationFile(mShopTrakTransactionsURLFileName, InformationType.ShopTrakTransactionsURL);

            mPieceWeightFileName = PieceWeightFileName;
            if (File.Exists(mRootDirectory.FullName + "\\" + mPieceWeightFileName))
                RetrieveInformationFromFile(mPieceWeightFileName, InformationType.PieceWeight);
            else
                CreateInformationFile(mPieceWeightFileName, InformationType.PieceWeight);

            mNetWeightAdjustmentFileName = NetWeightAdjustmentFileName;
            if (File.Exists(mRootDirectory.FullName + "\\" + mNetWeightAdjustmentFileName))
                RetrieveInformationFromFile(mNetWeightAdjustmentFileName, InformationType.NetWeightAdjustment);
            else
                CreateInformationFile(mNetWeightAdjustmentFileName, InformationType.NetWeightAdjustment);

            mBacklightColorFileName = BackgroundColorFileName;
            if (File.Exists(mRootDirectory.FullName + "\\" + mBacklightColorFileName))
                RetrieveInformationFromFile(mBacklightColorFileName, InformationType.ColorName);
            else
                CreateInformationFile(mBacklightColorFileName, InformationType.ColorName);
        }

        private void RetrieveInformationFromFile(string FileName, InformationType Information)
        {
            double dblTemp;
            string FilePathAndName = mRootDirectory.FullName + "\\" + FileName;

            
            Debug.Print("Reading file: " + FilePathAndName);
            try
                {
                    using (StreamReader objStreamReader = new StreamReader(FilePathAndName))
                        switch (Information)
                        {
                            case InformationType.LabelFormat:
                                LabelFormat = objStreamReader.ReadToEnd();
                                Debug.Print("LabelFormat Read from SD Card:\r\n" + LabelFormat);
                                break;
                            case InformationType.JobNumber:
                                JobNumber = objStreamReader.ReadLine().Trim();
                                Debug.Print("JobNumber Read from SD Card:\r\n" + JobNumber);
                                break;
                            case InformationType.OperationNumber:
                                Operation = objStreamReader.ReadLine().Trim();
                                Debug.Print("Operation Read from SD Card:\r\n" + Operation);
                                break;
                            case InformationType.ShopTrakTransactionsURL:
                                ShopTrakTransactionsURL = objStreamReader.ReadLine().Trim();
                                Debug.Print("ShopTrakTransactionsURL Read from SD Card:\r\n" + ShopTrakTransactionsURL);
                                break;
                            case InformationType.PieceWeight:
                                var strPieceWeight = objStreamReader.ReadLine().Trim();
                                PieceWeight = double.TryParse(strPieceWeight, out dblTemp) ? dblTemp : 0.0;
                                Debug.Print("PieceWeight Read from SD Card:\r\n" + PieceWeight);
                                break;
                            case InformationType.NetWeightAdjustment:
                                var strNetWeightAdjustment = objStreamReader.ReadLine().Trim();
                                NetWeightAdjustment = double.TryParse(strNetWeightAdjustment, out dblTemp) ? dblTemp : 0.0;
                                Debug.Print("NetWeightAdjustment Read from SD Card:\r\n" + NetWeightAdjustment);
                                break;
                            case InformationType.ColorName:
                                var strBackgroundColor = objStreamReader.ReadLine().Trim();
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
            string FilePathAndName = mRootDirectory.FullName + "\\" + FileName;
            string strContents = string.Empty;
            double dblContents = 0.0;
            BacklightColor enumBacklightColor = BacklightColor.White;


            Debug.Print("Creating  " + FilePathAndName + "...");
            using (var objFileStream = new FileStream(FilePathAndName, FileMode.Create))
            using (var objStreamWriter = new StreamWriter(objFileStream))
            {
                switch (Information)
                {
                    case InformationType.LabelFormat:
                        strContents = Label.DefaultLabel;
                        objStreamWriter.WriteLine(strContents);
                        objStreamWriter.WriteLine();
                        LabelFormat = strContents;
                        Debug.Print("Wrote contents: \r\n" + strContents + "\r\nTo File: " + FilePathAndName);
                        break;
                    case InformationType.JobNumber:
                        strContents = "B00123-000";
                        objStreamWriter.WriteLine(strContents);
                        objStreamWriter.WriteLine();
                        JobNumber = strContents;
                        Debug.Print("Wrote contents: \r\n" + strContents + "\r\nTo File: " + FilePathAndName);
                        break;
                    case InformationType.OperationNumber:
                        strContents = "10";
                        objStreamWriter.WriteLine(strContents);
                        objStreamWriter.WriteLine();
                        Operation = strContents;
                        Debug.Print("Wrote contents: \r\n" + strContents + "\r\nTo File: " + FilePathAndName);
                        break;
                    case InformationType.ShopTrakTransactionsURL:
                        strContents = "http://dataservice.wiretechfab.com:6156/SytelineDataService/ShopTrak/LCLTTransaction/Job=~p0&Suffix=~p1&Operation=~p2";
                        objStreamWriter.WriteLine(strContents);
                        objStreamWriter.WriteLine();
                        ShopTrakTransactionsURL = strContents;
                        Debug.Print("Wrote contents: \r\n" + strContents + "\r\nTo File: " + FilePathAndName);
                        break;
                    case InformationType.PieceWeight:
                        dblContents = 0.0000001;
                        objStreamWriter.WriteLine(strContents);
                        objStreamWriter.WriteLine();
                        PieceWeight = dblContents;
                        Debug.Print("Wrote contents: \r\n" + dblContents + "\r\nTo File: " + FilePathAndName);
                        break;
                    case InformationType.NetWeightAdjustment:
                        dblContents = 0.0;
                        objStreamWriter.WriteLine(strContents);
                        objStreamWriter.WriteLine();
                        NetWeightAdjustment = dblContents;
                        Debug.Print("Wrote contents: \r\n" + dblContents + "\r\nTo File: " + FilePathAndName);
                        break;
                    case InformationType.ColorName:
                        enumBacklightColor = BacklightColor.White;
                        objStreamWriter.WriteLine(enumBacklightColor);
                        objStreamWriter.WriteLine();
                        BacklightColor = enumBacklightColor;
                        Debug.Print("Wrote contents: \r\n" + enumBacklightColor + " (" + enumBacklightColor.GetColorName() + ")\r\nTo File: " + FilePathAndName);
                        break;
                }
                Debug.Print("Finished Write...");
            }
            
        }
    }
}
