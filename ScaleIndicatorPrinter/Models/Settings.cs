using System;
using Microsoft.SPOT;

using System.IO;
using NetMf.CommonExtensions;


namespace ScaleIndicatorPrinter.Models
{
    public class Settings
    {
        private static DirectoryInfo mRootDirectory { get; set; }
        public static DirectoryInfo RootDirectory { get { return mRootDirectory; } }


        private static string mLabelFormat { get; set; }
        public static string LabelFormat { get { return mLabelFormat; } }

        private static string mJobNumber { get; set; }
        public static string JobNumber { get { return mJobNumber; } }

        public static string Job { 
            get {
                if (mJobNumber.IndexOf('-') != -1)
                {
                    var intPlaceholder = mJobNumber.LastIndexOf('-');
                    return mJobNumber.Substring(0, intPlaceholder);
                }
                else
                    return "B";
            } 
        }

        public static int Suffix {
            get
            {
                try
                {
                    var intPlaceholder = mJobNumber.LastIndexOf('-');
                    return int.Parse(mJobNumber.Substring(intPlaceholder + 1, mJobNumber.Length - intPlaceholder - 1));
                }
                catch
                {
                    return int.MaxValue;
                }
            } 
        }

        private static string mOperationNumber { get; set; }
        public static int Operation { 
            get {
                try {
                    return int.Parse(mOperationNumber);
                }
                catch {
                    return int.MaxValue;
                }
                 
            } 
        }

        private static string mShopTrakTransactionsURL { get; set; }
        public static string ShopTrakTransactionsURL { get { return mShopTrakTransactionsURL; } }

        private static string mPieceWeight { get; set; }
        public static double PieceWeight { 
            get {
                double dblTemp;

                return double.TryParse(mPieceWeight, out dblTemp) ? dblTemp : 0.0;
            } 
        }


        public Settings(DirectoryInfo objDirectory )
        {
            if (objDirectory.Exists)
                mRootDirectory = objDirectory;
            else
            {
                Debug.Print(objDirectory.FullName + " is not a Valid Directory!!");
                throw new ApplicationException(objDirectory.FullName + " is not a Valid Directory!!");
            }   
        }

        public void RetrieveInformationFromFile(string FileName, InformationType Information)
        {
            string FilePathAndName = RootDirectory.FullName + "\\" + FileName;
            string[] InformationTypes = new [] { "Label Format", "Job", "Operation", "ShopTrak Transactions URL", "Piece Weight" };


            if (File.Exists(FilePathAndName))
                using (StreamReader objStreamReader = new StreamReader(FilePathAndName))
                    switch (Information)
                    {
                        case InformationType.LabelFormat:
                            //mLabelFormat = "N" + "\r\n" +
                            //"A50,100,0,5,1,1,N,\"EXAMPLE 1\"" + "\r\n" +
                            //"B50,150,0,3,3,7,200,B,\"EXAMPLE 1\"" + "\r\n" +
                            //"P1" + "\r\n";
                            mLabelFormat = objStreamReader.ReadToEnd();
                            break;
                        case InformationType.JobNumber:
                            mJobNumber = objStreamReader.ReadLine().Trim();
                            break;
                        case InformationType.OperationNumber:
                            mOperationNumber = objStreamReader.ReadLine().Trim();
                            break;
                        case InformationType.ShopTrakTransactionsURL:
                            mShopTrakTransactionsURL = objStreamReader.ReadLine().Trim();
                            break;
                        case InformationType.PieceWeight:
                            mPieceWeight = objStreamReader.ReadLine().Trim();
                            break;
                    }
            else
            {
                Debug.Print(FileName + " is not a Valid " + InformationTypes[(int)Information] + " Data File!!");
                throw new ApplicationException(FileName + " is not a Valid " + InformationTypes[(int)Information] + " Data File!!");
            }
                
        }

        public void SetJobNumber(string FileName, string Job)
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + FileName))
                objStreamWriter.WriteLine(Job);

            mJobNumber = Job;
        }

        public void SetOperationNumber(string FileName, string Operation)
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + FileName))
                objStreamWriter.WriteLine(Operation);

            mOperationNumber = Operation;
        }

        public void SetPieceWeight(string FileName, double PieceWeight)
        {
            using (StreamWriter objStreamWriter = new StreamWriter(RootDirectory.FullName + "\\" + FileName))
                objStreamWriter.WriteLine(PieceWeight);

            mPieceWeight = PieceWeight.ToString();
        }
    }
}
