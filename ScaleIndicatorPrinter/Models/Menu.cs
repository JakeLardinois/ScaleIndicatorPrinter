using System;
using Microsoft.SPOT;

using NetduinoRGBLCDShield;


namespace ScaleIndicatorPrinter.Models
{
    public class Menu
    {
        private RGBLCDShield lcdBoard { get; set; }

        public Menu(MCP23017 mcp23017) 
        {
            lcdBoard = new RGBLCDShield(mcp23017);
        }
        
        public RecievedData DataRecieved { get; set; }

        private int intMenuSelection { get; set; }
        private int AvailableMenuCount { get { return 8; } } //this represents 8 available menus. Note that the MenuSelection enumeration has 12 available values but I only want to be able to cycle 
        public MenuSelection MenuSelection                  //through 8 of them since the other 3 AdjustPieceWeight, AdjustNetWeight & ChangeColor are set via the 'Select' Button.
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
            Debug.Print("Going to Next Available Menu Selection...");
            intMenuSelection = ++intMenuSelection % AvailableMenuCount;
            Debug.Print("Current Menu Selection is: " + intMenuSelection);
        }

        public void GoToPreviousAvailableMenuSelection()
        {
            Debug.Print("Going to Previous Available Menu Selection...");
            if (intMenuSelection == 0)
            {
                Debug.Print("Set Menu Selection to 0...");
                intMenuSelection = AvailableMenuCount;
            }
            intMenuSelection = --intMenuSelection % AvailableMenuCount;
            Debug.Print("Current Menu Selection is: " + intMenuSelection);
        }

        public void DisplayInformation(Settings objSettings)
        {
            lcdBoard.Clear();
            lcdBoard.SetPosition(0, 0);

            Debug.Print("Updating Menu display for Menu Selection: " + MenuSelection);
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
                    lcdBoard.Write("Net Wt Adjustment:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.NetWeightAdjustment.ToString("F3"));
                    break;
                case MenuSelection.ViewBackgroundColor:
                    lcdBoard.Write("Background Color:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.BackgroundColorName);
                    break;
                case MenuSelection.ViewNetworkInfo:
                    lcdBoard.Write("View Network Info");
                    lcdBoard.ClearRow(1);
                    break;
                case MenuSelection.Reboot:
                    lcdBoard.Write("Reboot Device");
                    lcdBoard.ClearRow(1);
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
                case MenuSelection.ChangeBackgroundColor:
                    lcdBoard.Write("Change Color...");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.BackgroundColorName);
                    break;
                case MenuSelection.DisplayNetworkInfo:
                    lcdBoard.Write(objSettings.IPAddress);
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.NetMask);
                    break;
                case MenuSelection.Rebooting:
                    lcdBoard.Write("Goodbye");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write("I'm Rebooting...");
                    break;
            }
            Debug.Print("Finished Display Update...");
        }

        public void DisplayError(Exception objEx)
        {
            var strExceptionType = objEx.GetType().FullName;
            lcdBoard.Clear();
            lcdBoard.SetPosition(0, 0);


            Debug.Print("Displaying Exception...");
            if (objEx.Message != null) //Write the Exception Message to the LCD Display...
            {
                Debug.Print("Writing Exception Message to the display: " + objEx.Message);
                lcdBoard.Write("ERR-" + objEx.Message.Substring(0, objEx.Message.Length - 1));
                if (objEx.Message.Length >= 13)
                {
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objEx.Message.Substring(12, objEx.Message.Length - 12));
                }
            }
            else //write the exception type out to the LCD Display...
            {
                Debug.Print("No Exception Message, writing Exception Type to the Display: " + strExceptionType);
                lcdBoard.Write(strExceptionType.Substring(0, strExceptionType.Length - 1));
                if (strExceptionType.Length >= 16)
                {
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(strExceptionType.Substring(16, strExceptionType.Length - 16));
                }
            }

        }

        public void SetBackLightColor(BacklightColor color)
        {
            Debug.Print("Changing Background color to " + color.GetColorName());
            lcdBoard.SetBacklight(color);
        }
    }
}
