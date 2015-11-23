using System;
using Microsoft.SPOT;

using NetduinoRGBLCDShield;


namespace ScaleIndicatorPrinter.Models
{
    public class Menu
    {
        public RGBLCDShield lcdBoard { get; set; }
        public RecievedData DataRecieved { get; set; }

        private int intMenuSelection { get; set; }
        private int AvailableMenuCount { get { return 5; } } //this represents 5 available menus. Note that the MenuSelection enum has 7 available values but I only want to be able to cycle 
        public MenuSelection MenuSelection                  //through 5 of them since the other 2 AdjustPieceWeight & AdjustNetWeight are set via the 'Select' Button.
        {
            get
            {
                return (MenuSelection)System.Math.Abs(intMenuSelection);
            }
            set
            {
                intMenuSelection = (int)value;
            }
        }

        public void GoToNextAvailableMenuSelection()
        {
            intMenuSelection = ++intMenuSelection % AvailableMenuCount;
        }

        public void GoToPreviousAvailableMenuSelection()
        {
            intMenuSelection = --intMenuSelection % AvailableMenuCount;
        }

        public void DisplayInformation(Settings objSettings)
        {
            lcdBoard.Clear();
            lcdBoard.SetPosition(0, 0);

            
            switch (MenuSelection)
            {
                case MenuSelection.PrintLabel:
                    DataRecieved = RecievedData.ScaleIndicator;
                    lcdBoard.Write("Press Print...");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write("To print a label");
                    break;
                case MenuSelection.Job:
                    if (DataRecieved == RecievedData.ScannerJobAndSuffix)
                        lcdBoard.Write("Scan Job #...");
                    else
                        lcdBoard.Write("Job:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.JobNumber);
                    break;
                case MenuSelection.Operation:
                    if (DataRecieved == RecievedData.ScannerOperation)
                        lcdBoard.Write("Scan Op #...");
                    else
                        lcdBoard.Write("Operation:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.Operation.ToString());
                    break;
                case MenuSelection.ViewPieceWeight:
                    lcdBoard.Write("Piece Weight:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.PieceWeight.ToString("F3"));
                    break;
                case MenuSelection.ViewNetWeightAdjustment:
                    lcdBoard.Write("Net Weight Adjustment:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.NetWeightAdjustment.ToString("F3"));
                    break;
                case MenuSelection.AdjustPieceWeight:
                    lcdBoard.Write("Adj Pc Weight...");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.PieceWeight.ToString("F3"));
                    break;
                case MenuSelection.AdjustNetWeight:
                    lcdBoard.Write("Adj Net Weight...");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.NetWeightAdjustment.ToString("F3"));
                    break;
            }
        }

        public void DisplayError(Exception objEx)
        {
            var strExceptionType = objEx.GetType().FullName;
            lcdBoard.Clear();
            lcdBoard.SetPosition(0, 0);

            if (objEx.Message != null) //Write the Exception Message to the LCD Display...
            {
                lcdBoard.Write("ERR-" + objEx.Message.Substring(0, objEx.Message.Length - 1));
                if (objEx.Message.Length >= 13)
                {
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objEx.Message.Substring(12, objEx.Message.Length - 12));
                }
            }
            else //write the exception type out to the LCD Display...
            {
                lcdBoard.Write(strExceptionType.Substring(0, strExceptionType.Length - 1));
                if (strExceptionType.Length >= 16)
                {
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(strExceptionType.Substring(16, strExceptionType.Length - 16));
                }
            }

        }
    }
}
