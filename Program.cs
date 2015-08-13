using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

using ScaleIndicatorPrinter.Models;
using System.IO.Ports;
using Toolbox.NETMF;


namespace ScaleIndicatorPrinter
{
    public class Program
    {
        private static MySerialPort mIndicatorScannerSerialPort;
        private static MySerialPort mPrinterSerialPort;

        public static MCP23017 mcp23017 { get; set; }
        public static RGBLCDShield lcdBoard { get; set; }

        private static RecievedData mDataRecieved { get; set; }
        private static Settings mSettings { get; set; }

        public static string mShopTrakTransactionsURL { get; set; }

        private static string mRootDirectory { get { return @"\SD\"; } }
        private static string mLabelFormatFileName { get { return "LabelFormat.txt"; } }
        private static string mJobFileName { get { return "Job.txt"; } }
        private static string mOperationFileName { get { return "Operation.txt"; } }
        private static string mShopTrakTransactionsURLFileName { get { return "ShopTrakTransactionsURL.txt"; } }

        private static int mintMenuSelection { get; set; }
        private static int mMenuSelection { get { return System.Math.Abs(mintMenuSelection); } }

        private static InterruptPort btnBoard { get; set; }
        private static OutputPort onboardLED = new OutputPort(Pins.ONBOARD_LED, false);

        private static InterruptPort btnShield { get; set; }

        public static void Main()
        {
            // initialize the serial port for COM1 (using D0 & D1) and COM2 (using D2 & D3)
            mIndicatorScannerSerialPort = new MySerialPort(SerialPorts.COM2, BaudRate.Baudrate9600, Parity.None, DataBits.Eight, StopBits.One);
            mPrinterSerialPort = new MySerialPort(SerialPorts.COM3, BaudRate.Baudrate9600, Parity.None, DataBits.Eight, StopBits.One);

            // open the serial-ports, so we can send & receive data
            mIndicatorScannerSerialPort.Open();
            mPrinterSerialPort.Open();

            // add an event-handler for handling incoming data
            mIndicatorScannerSerialPort.DataReceived += new SerialDataReceivedEventHandler(IndicatorScannerSerialPort_DataReceived);

            //InterruptEdgeLevelLow only fires the event the first time that the button descends
            btnBoard = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh);
            // Create an event handler for the button
            btnBoard.OnInterrupt += new NativeEventHandler(btnBoard_OnInterrupt);

            mSettings = new Settings(new System.IO.DirectoryInfo(mRootDirectory));

            mSettings.RetrieveInformationFromFile(mLabelFormatFileName, InformationType.LabelFormat);
            Label.LabelFormat = Settings.LabelFormat;

            //mSettings.SetJobNumber(mJobFileName, "B000053070-0000");
            mSettings.RetrieveInformationFromFile(mJobFileName, InformationType.JobNumber);

            //mSettings.SetOperationNumber(mOperationFileName, "10");
            mSettings.RetrieveInformationFromFile(mOperationFileName, InformationType.OperationNumber);


            mSettings.RetrieveInformationFromFile(mShopTrakTransactionsURLFileName, InformationType.ShopTrakTransactionsURL);
            mShopTrakTransactionsURL = Settings.ShopTrakTransactionsURL;


            // the MCP is what allows us to talk with the RGB LCD panel
            mcp23017 = new MCP23017();
            // and this is a class to help us chat with the LCD panel
            lcdBoard = new RGBLCDShield(mcp23017);

            // we'll follow the Adafruit example code
            mintMenuSelection = (int)MenuSelection.PrintLabel;
            DisplayInformation();


            // Setup the interrupt port
            btnShield = new InterruptPort(Pins.GPIO_PIN_D10, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);
            // Bind the interrupt handler to the pin's interrupt event.
            btnShield.OnInterrupt += new NativeEventHandler(btnShield_OnInterrupt);


            // we are done
            Thread.Sleep(Timeout.Infinite);

            //while (true) //Thread.Sleep(Timeout.Infinite);
            //{
            //    ScaleIndicatorPrinter.Models.Button Button = lcdBoard.ReadButtons();

            //    switch (Button)
            //    {
            //        case Models.Button.Left:
            //            --mintMenuSelection;
            //            Thread.Sleep(100); //I had to add a pause here or else the loop would execute multiple times during a button press...
            //            DisplayInformation();
            //            break;
            //        case Models.Button.Right:
            //            ++mintMenuSelection;
            //            Thread.Sleep(100);
            //            DisplayInformation();
            //            break;
            //        case Models.Button.Select:
            //            Thread.Sleep(100);
            //            PerformAction();
            //            break;
            //    }
            //}
        }

        public static void btnShield_OnInterrupt(UInt32 data1, UInt32 data2, DateTime time)
        {
            // Set the LED to its new state.
            //onboardLED.Write(!onboardLED.Read());
            
            
            //var blah2 = mcp23017.DigitalRead((byte)ScaleIndicatorPrinter.Models.MCP23017.Command.MCP23017_INTCAPA);

            
            //var strHexValue = Tools.Dec2Hex(ButtonPressed, 2); //convert the ushort to hexadecimal string
            //var strButtonHexValue = strHexValue.Substring(strHexValue.Length - 2, 2); //the button values are the last 2 characters of the hex string
            //var intButtonValue = Tools.Dec2Hex(Int32.Parse(strButtonHexValue));
            //var blah = (int)ScaleIndicatorPrinter.Models.Button.Left;

            
            //switch (ButtonPressed)
            //{
            //    case 34304:// 0x8600
            //        break;
            //    case 34305:// 0x8601, which corresponds to 0x01 == Select

            //        break;
            //    case 34306:// 0x8602, which corresponds to 0x02 == Right
            //        ++mintMenuSelection;
            //        DisplayInformation();
            //        break;
            //    case 34308:// 0x8604, which corresponds to 0x04 == Down

            //        break;
            //    case 34320:// 0x8610, which corresponds to 0x10 == Left
            //        --mintMenuSelection;
            //        DisplayInformation();
            //        break;
            //    case 34312:// 0x8608, which corresponds to 0x08 == Up

            //        break;
            //}

            /*For some reason this event returns data multiple times when a button gets pressed. If you wait a little bit in the debugger, then the variable actually eventually changes to a value that contains a button's value.
             * What I first observed was that a number such as 34304 was returned multiple times before a number such as 34305 would be returned. 34304=0x8600=NoButton, 34305=0x8601=SelectButton (which was indicated as 0x01 in the 
             * Select button Enum). So I initially designed the loop to check for 34304, 34305, 34306, etc. to determine which button was pressed.  However, when I plugged in a different LED Shield, I noticed that the numbers changed
             * 
             */
            var ButtonPressed = mcp23017.ReadGpioAB();

            var InterruptBits = BitConverter.GetBytes(ButtonPressed);
            switch (InterruptBits[0]) //the 0 value contains the button that was pressed...
            {
                case (int)ScaleIndicatorPrinter.Models.Button.Left:
                    --mintMenuSelection;
                    DisplayInformation();
                    break;
                case (int)ScaleIndicatorPrinter.Models.Button.Right:
                    ++mintMenuSelection;
                    DisplayInformation();
                    break;
                case (int)ScaleIndicatorPrinter.Models.Button.Up:
                    break;
                case (int)ScaleIndicatorPrinter.Models.Button.Down:
                    break;
                case (int)ScaleIndicatorPrinter.Models.Button.Select:
                    PerformAction();
                    break;
            }
        }

        private static void btnBoard_OnInterrupt(uint port, uint data, DateTime time)
        {
            for (int intCounter = 2; intCounter < 7; intCounter++)
            {
                onboardLED.Write(intCounter % 2 == 1);
                Thread.Sleep(500);
            }

            //DisplayInformation();
            SerialDataReceivedEventArgs objSerialDataReceivedEventArgs = null;
            IndicatorScannerSerialPort_DataReceived(new object(), objSerialDataReceivedEventArgs);
        }

        private static void PerformAction()
        {
            lcdBoard.SetPosition(0, 0);

            switch ((int)mMenuSelection % 4)
            {
                case (int)MenuSelection.Job:
                    mDataRecieved = RecievedData.ScannerJobAndSuffix;
                    lcdBoard.Write("Scan Job #...");
                    break;
                case (int)MenuSelection.Operation:
                    mDataRecieved = RecievedData.ScannerOperation;
                    lcdBoard.Write("Scan Op #...");
                    break;
            }
        }

        private static void DisplayInformation()
        {
            lcdBoard.Clear();
            lcdBoard.SetPosition(0, 0);


            switch (mMenuSelection % 4)
            {
                case (int)MenuSelection.PrintLabel:
                    mDataRecieved = RecievedData.ScaleIndicator;
                    lcdBoard.Write("Press Print...");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write("To print a label");
                    break;
                case (int)MenuSelection.Job :
                    mDataRecieved = RecievedData.None;
                    lcdBoard.Write("Job:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(Settings.JobNumber);
                    break;
                case (int)MenuSelection.Operation:
                    mDataRecieved = RecievedData.None;
                    lcdBoard.Write("Operation:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(Settings.OperationNumber);
                    break;
                case (int)MenuSelection.Employees:
                    mDataRecieved = RecievedData.None;
                    lcdBoard.Write("Employees:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(Settings.OperationNumber);
                    break;
            }
        }

        private static void IndicatorScannerSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var strMessage = mIndicatorScannerSerialPort.ReadString();
            //var strMessage = "B000053350-0000\r\n";

            switch (mDataRecieved)
            {
                case RecievedData.ScaleIndicator:
                    var objIndicatorData = new IndicatorData(strMessage);
                    
                    if (objIndicatorData.HasValidDataString)
                    {
                        var objLabel = new Label(new string[] { objIndicatorData.GrossWeight.ToString(), objIndicatorData.NetWeight.ToString() });
                        mPrinterSerialPort.WriteString(objLabel.LabelText);
                    }
                    break;
                case RecievedData.ScannerJobAndSuffix:
                    var str = strMessage;
                    DisplayInformation();
                    break;
            }

            
        }
    }
}
