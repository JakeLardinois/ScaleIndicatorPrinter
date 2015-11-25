using System;
using Microsoft.SPOT;

using System.IO;
using NetMf.CommonExtensions;
using NetduinoRGBLCDShield;

namespace ScaleIndicatorPrinter.Models
{
    public class Settings
    {
        public string RootDirectoryPath { get; set; }
        private DirectoryInfo RootDirectory { get; set; }

        public string LabelFormatFileName { get; set; }
        public string LabelFormat { get; set; }
        public void StoreLabelFormat()
        {
            string strFilePathAndName = RootDirectory.FullName + "\\" + LabelFormatFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(LabelFormat);
            Debug.Print("Wrote Contents: " + LabelFormat + "\r\nTo File: " + strFilePathAndName);
        }

        public string JobNumberFileName { get; set; }
        public string JobNumber { get; set; }
        public void StoreJobNumber()
        {
            string strFilePathAndName = RootDirectory.FullName + "\\" + JobNumberFileName;
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

        public string OperationFileName { get; set; }
        public string Operation { get; set; }
        public void StoreOperationNumber()
        {
            string strFilePathAndName = RootDirectory.FullName + "\\" + OperationFileName;
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

        public string ShopTrakTransactionsURLFileName { get; set; }
        public string ShopTrakTransactionsURL { get; set; }
        public void StoreShopTrakTransactionsURL()
        {
            string strFilePathAndName = RootDirectory.FullName + "\\" + ShopTrakTransactionsURLFileName;
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

        public string PieceWeightFileName { get; set; }
        private double mdblPieceWeight { get; set; }
        public double PieceWeight { 
            get { return mdblPieceWeight; } 
            set { mdblPieceWeight = value > 0 ? value : 0; } 
        }
        public void StorePieceWeight()
        {
            string strFilePathAndName = RootDirectory.FullName + "\\" + PieceWeightFileName;
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

        public string NetWeightAdjustmentFileName { get; set; }
        public double NetWeightAdjustment{ get; set; }
        public void StoreNetWeightAdjustment()
        {
            string strFilePathAndName = RootDirectory.FullName + "\\" + NetWeightAdjustmentFileName;
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

        public string BackgroundColorFileName { get; set; }
        private int intBackGroundColor { get; set; }
        public BacklightColor BackgroundColor {
            get { return (BacklightColor)intBackGroundColor; }
            set { intBackGroundColor = (int)value; }
        }
        public string BackgroundColorName { get { return BackgroundColor.GetColorName(); } }
        public void StoreBackgroundColor()
        {
            string strFilePathAndName = RootDirectory.FullName + "\\" + BackgroundColorFileName;
            using (StreamWriter objStreamWriter = new StreamWriter(strFilePathAndName))
                objStreamWriter.WriteLine(BackgroundColor);
            Debug.Print("Wrote Contents: " + BackgroundColor + "\r\nTo File: " + strFilePathAndName);
        }
        public void NextBackgroundColor()
        {
            intBackGroundColor = ++intBackGroundColor % (int)BacklightColor.ColorCount;
        }
        public void PreviousBackgroundColor()
        {
            if (intBackGroundColor == 0)
                intBackGroundColor = (int)BacklightColor.ColorCount;
            intBackGroundColor = --intBackGroundColor % (int)BacklightColor.ColorCount;
        }

        public void RetrieveSettingsFromSDCard()
        {
            RootDirectory = new System.IO.DirectoryInfo(RootDirectoryPath);
            if (!RootDirectory.Exists)
            {
                Debug.Print(RootDirectory.FullName + " is not a Valid Directory!!");
                throw new ApplicationException(RootDirectory.FullName + " is not a Valid Directory!!");
            }

            RetrieveInformationFromFile(LabelFormatFileName, InformationType.LabelFormat);
            RetrieveInformationFromFile(JobNumberFileName, InformationType.JobNumber);
            RetrieveInformationFromFile(OperationFileName, InformationType.OperationNumber);
            RetrieveInformationFromFile(ShopTrakTransactionsURLFileName, InformationType.ShopTrakTransactionsURL);
            RetrieveInformationFromFile(PieceWeightFileName, InformationType.PieceWeight);
            RetrieveInformationFromFile(NetWeightAdjustmentFileName, InformationType.NetWeightAdjustment);
            RetrieveInformationFromFile(BackgroundColorFileName, InformationType.ColorName);
        }

        private void RetrieveInformationFromFile(string FileName, InformationType Information)
        {
            double dblTemp;
            string FilePathAndName = RootDirectory.FullName + "\\" + FileName;
            string[] InformationTypes = new [] { "Label Format", "Job", "Operation", "ShopTrak Transactions URL", "Piece Weight", "Net Weight Adjustment", "Background Color" };

            Debug.Print("Reading file: " + FilePathAndName);
            if (File.Exists(FilePathAndName))
                using (StreamReader objStreamReader = new StreamReader(FilePathAndName))
                    switch (Information)
                    {
                        case InformationType.LabelFormat:
                            LabelFormat = objStreamReader.ReadToEnd();
                            Debug.Print("LabelFormat from SD Card: " + LabelFormat);
                            break;
                        case InformationType.JobNumber:
                            JobNumber = objStreamReader.ReadLine().Trim();
                            Debug.Print("JobNumber from SD Card: " + JobNumber);
                            break;
                        case InformationType.OperationNumber:
                            Operation = objStreamReader.ReadLine().Trim();
                            Debug.Print("Operation from SD Card: " + Operation);
                            break;
                        case InformationType.ShopTrakTransactionsURL:
                            ShopTrakTransactionsURL = objStreamReader.ReadLine().Trim();
                            Debug.Print("ShopTrakTransactionsURL from SD Card: " + ShopTrakTransactionsURL);
                            break;
                        case InformationType.PieceWeight:
                            var strPieceWeight = objStreamReader.ReadLine().Trim();
                            PieceWeight = double.TryParse(strPieceWeight, out dblTemp) ? dblTemp : 0.0;
                            Debug.Print("PieceWeight from SD Card: " + PieceWeight);
                            break;
                        case InformationType.NetWeightAdjustment:
                            var strNetWeightAdjustment = objStreamReader.ReadLine().Trim();
                            NetWeightAdjustment = double.TryParse(strNetWeightAdjustment, out dblTemp) ? dblTemp : 0.0;
                            Debug.Print("NetWeightAdjustment from SD Card: " + NetWeightAdjustment);
                            break;
                        case InformationType.ColorName:
                            var strBackgroundColor = objStreamReader.ReadLine().Trim();
                            BackgroundColor = double.TryParse(strBackgroundColor, out dblTemp) ? (BacklightColor)dblTemp : (BacklightColor)0;
                            Debug.Print("BackgroundColor from SD Card: " + BackgroundColor);
                            break;
                    }
            else
            {
                string strContents = string.Empty;

                Debug.Print(FilePathAndName + " is not a Valid " + InformationTypes[(int)Information] + " Data File!!");
                Debug.Print("Creating  " + FilePathAndName + "...");
                using (var objFileStream = new FileStream(FilePathAndName, FileMode.Create))
                    using (var objStreamWriter = new StreamWriter(objFileStream))
                        switch(Information)
                        {
                            case InformationType.LabelFormat:
                                strContents = Label.DefaultLabel;
                                objStreamWriter.WriteLine(strContents);
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.JobNumber:
                                strContents = "B00123-000";
                                objStreamWriter.WriteLine("0");
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.OperationNumber:
                                strContents = "10";
                                objStreamWriter.WriteLine(strContents);
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.ShopTrakTransactionsURL:
                                strContents = "http://dataservice.wiretechfab.com:6156/SytelineDataService/ShopTrak/LCLTTransaction/Job=~p0&Suffix=~p1&Operation=~p2";
                                objStreamWriter.WriteLine(strContents);
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.PieceWeight:
                                strContents = "0";
                                objStreamWriter.WriteLine(strContents);
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.NetWeightAdjustment:
                                strContents = "0";
                                objStreamWriter.WriteLine(strContents);
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.ColorName:
                                strContents = "7";// 7 is White...
                                objStreamWriter.WriteLine(strContents); 
                                objStreamWriter.WriteLine();
                                break;
                        }
                System.Threading.Thread.Sleep(100);
                Debug.Print("Created " + FileName + " for a " + InformationTypes[(int)Information] + " Data File...\r\n" +
                    "Wrote to File: " + strContents);
                throw new ApplicationException(FileName + " is not a Valid " + InformationTypes[(int)Information] + " Data File!!");
            }
                
        }
    }
}
