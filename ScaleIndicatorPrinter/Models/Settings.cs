using System;
using Microsoft.SPOT;

using System.IO;
using NetMf.CommonExtensions;


namespace ScaleIndicatorPrinter.Models
{
    public class Settings
    {
        public string RootDirectoryPath { get; set; }
        private DirectoryInfo RootDirectory { get; set; }

        public string LabelFormatFileName { get; set; }
        public string LabelFormat { get; set; }

        public string JobNumberFileName { get; set; }
        public string JobNumber { get; set; }
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

        public string PieceWeightFileName { get; set; }
        public double PieceWeight { get; set; }

        public string NetWeightAdjustmentFileName { get; set; }
        public double NetWeightAdjustment{ get; set; }


        public void RetrieveSettingsFromSDCard()
        {
            RootDirectory = new System.IO.DirectoryInfo(RootDirectoryPath);
            if (!RootDirectory.Exists)
            {
                Debug.Print(RootDirectory.FullName + " is not a Valid Directory!!");
                throw new ApplicationException(RootDirectory.FullName + " is not a Valid Directory!!");
            }

            //SetLabelFormat(mLabelFormatFileName, Label.SampleLabel);
            RetrieveInformationFromFile(LabelFormatFileName, InformationType.LabelFormat);

            //SetJobNumber(mJobFileName, "B000053070-0000");
            RetrieveInformationFromFile(JobNumberFileName, InformationType.JobNumber);

            //SetOperationNumber(mOperationFileName, "10");
            RetrieveInformationFromFile(OperationFileName, InformationType.OperationNumber);

            //SetShopTrakTransactionsURL(mShopTrakTransactionsURLFileName, 
            //    "http://dataservice.wiretechfab.com:6156/SytelineDataService/ShopTrak/LCLTTransaction/Job=~p0&Suffix=~p1&Operation=~p2");
            RetrieveInformationFromFile(ShopTrakTransactionsURLFileName, InformationType.ShopTrakTransactionsURL);

            //SetPieceWeight(mPieceWeightFileName, .5);
            RetrieveInformationFromFile(PieceWeightFileName, InformationType.PieceWeight);

            //SetNetWeightAdjustment(mNetWeightAdjustmentFileName, 10);
            RetrieveInformationFromFile(NetWeightAdjustmentFileName, InformationType.NetWeightAdjustment);
        }

        private void RetrieveInformationFromFile(string FileName, InformationType Information)
        {
            double dblTemp;

            string FilePathAndName = RootDirectory.FullName + "\\" + FileName;
            string[] InformationTypes = new [] { "Label Format", "Job", "Operation", "ShopTrak Transactions URL", "Piece Weight", "Net Weight Adjustment" };


            if (File.Exists(FilePathAndName))
                using (StreamReader objStreamReader = new StreamReader(FilePathAndName))
                    switch (Information)
                    {
                        case InformationType.LabelFormat:
                            LabelFormat = objStreamReader.ReadToEnd();
                            break;
                        case InformationType.JobNumber:
                            JobNumber = objStreamReader.ReadLine().Trim();
                            break;
                        case InformationType.OperationNumber:
                            Operation = objStreamReader.ReadLine().Trim();
                            break;
                        case InformationType.ShopTrakTransactionsURL:
                            ShopTrakTransactionsURL = objStreamReader.ReadLine().Trim();
                            break;
                        case InformationType.PieceWeight:
                            var strPieceWeight = objStreamReader.ReadLine().Trim();
                            PieceWeight = double.TryParse(strPieceWeight, out dblTemp) ? dblTemp : 0.0;
                            break;
                        case InformationType.NetWeightAdjustment:
                            var strNetWeightAdjustment = objStreamReader.ReadLine().Trim();
                            NetWeightAdjustment = double.TryParse(strNetWeightAdjustment, out dblTemp) ? dblTemp : 0.0;
                            break;
                    }
            else
            {
                Debug.Print(FileName + " is not a Valid " + InformationTypes[(int)Information] + " Data File!!");
                using (var objFileStream = new FileStream(FilePathAndName, FileMode.Create))
                    using (var objStreamWriter = new StreamWriter(objFileStream))
                        switch(Information)
                        {
                            case InformationType.LabelFormat:
                                objStreamWriter.WriteLine(Label.DefaultLabel);
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.JobNumber:
                                objStreamWriter.WriteLine("0");
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.OperationNumber:
                                objStreamWriter.WriteLine("0");
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.ShopTrakTransactionsURL:
                                objStreamWriter.WriteLine("0");
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.PieceWeight:
                                objStreamWriter.WriteLine("0");
                                objStreamWriter.WriteLine();
                                break;
                            case InformationType.NetWeightAdjustment:
                                objStreamWriter.WriteLine("0");
                                objStreamWriter.WriteLine();
                                break;
                        }
                Debug.Print("Created " + FileName + " for a " + InformationTypes[(int)Information] + " Data File...");
                throw new ApplicationException(FileName + " is not a Valid " + InformationTypes[(int)Information] + " Data File!!");
            }
                
        }

        public void StoreLabelFormat()
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + LabelFormatFileName))
                objStreamWriter.WriteLine(Job);
        }

        public void StoreJobNumber()
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + JobNumberFileName))
                objStreamWriter.WriteLine(Job);
        }

        public void StoreOperationNumber()
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + OperationFileName))
                objStreamWriter.WriteLine(Operation);
        }

        public void StoreShopTrakTransactionsURL()
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + ShopTrakTransactionsURLFileName))
                objStreamWriter.WriteLine(Job);
        }

        public void StorePieceWeight()
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + PieceWeightFileName))
                objStreamWriter.WriteLine(PieceWeight);
        }

        public void StoreNetWeightAdjustment()
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + NetWeightAdjustmentFileName))
                objStreamWriter.WriteLine(NetWeightAdjustment);
        }
    }
}
