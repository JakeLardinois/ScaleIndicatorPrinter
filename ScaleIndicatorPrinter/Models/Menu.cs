using System;
using Microsoft.SPOT;

using NetduinoRGBLCDShield;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;


namespace ScaleIndicatorPrinter.Models
{
    public class Menu
    {
        public Menu()
        {
            // the MCP is what allows us to talk with the RGB LCD panel
            mcp23017 = new MCP23017();
            // and this is a class to help us chat with the LCD panel
            lcdBoard = new RGBLCDShield(mcp23017);

            OutputPort DigitalPin9 = new OutputPort(Pins.GPIO_PIN_D9, false); //Goes to S0
            OutputPort DigitalPin10 = new OutputPort(Pins.GPIO_PIN_D10, false);//Goes to S1
            OutputPort DigitalPin11 = new OutputPort(Pins.GPIO_PIN_D11, false);//Goes to S2
            OutputPort DigitalPin12 = new OutputPort(Pins.GPIO_PIN_D12, false);//Goes to S3

            Mux = new CD74HC4067(DigitalPin9, DigitalPin10, DigitalPin11, DigitalPin12);
            
        }

        public MCP23017 mcp23017 { get; set; }
        public RGBLCDShield lcdBoard { get; set; }

        public CD74HC4067 Mux { get; set; }

        public RecievedData DataRecieved { get; set; }

        public int intMenuSelection { get; set; }
        private int AvailableMenuCount { get { return 5; } } //this represents 5 available menus. Note that the MenuSelection enum has 7 available values but I only want to be able to cycle 
        public MenuSelection AvailableMenuSelection          //through 5 of them since the other 2 AdjustPieceWeight & AdjustNetWeight are set via the 'Select' Button.
        {
            get
            {
                return (MenuSelection)System.Math.Abs(intMenuSelection % AvailableMenuCount);
            }
        }
        public MenuSelection MenuSelection
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

        private int mintIncrementSelection { get; set; }
        public int IncrementSelection { 
            get { 
                return System.Math.Abs(mintIncrementSelection); 
            }
            set
            {
                mintIncrementSelection = value;
            }
        }
        public double[] Increments { get; set; }

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
                    //Tell MUX what channel to listen on...
                    Mux.SetPort(MuxChannel.C0);
                    break;
                case MenuSelection.Job:
                    DataRecieved = RecievedData.None;
                    lcdBoard.Write("Job:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.JobNumber);
                    //Tell MUX what channel to listen on...
                    Mux.SetPort(MuxChannel.C1);
                    break;
                case MenuSelection.Operation:
                    DataRecieved = RecievedData.None;
                    lcdBoard.Write("Operation:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.Operation.ToString());
                    //Tell MUX what channel to listen on...
                    Mux.SetPort(MuxChannel.C1);
                    break;
                case MenuSelection.ViewPieceWeight:
                    DataRecieved = RecievedData.None;
                    lcdBoard.Write("Piece Weight:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.PieceWeight.ToString("F3"));
                    break;
                case MenuSelection.ViewNetWeightAdjustment:
                    DataRecieved = RecievedData.None;
                    lcdBoard.Write("Net Weight Adjustment:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(objSettings.NetWeightAdjustment.ToString("F3"));
                    break;
            }
        }

        public void DisplayPieceWeight(Settings objSettings)
        {
            lcdBoard.ClearRow(1);
            lcdBoard.Write(objSettings.PieceWeight.ToString("F3"));
        }

        public void DisplayNetWeightAdjustment(Settings objSettings)
        {
            lcdBoard.ClearRow(1);
            lcdBoard.Write(objSettings.NetWeightAdjustment.ToString("F3"));
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
