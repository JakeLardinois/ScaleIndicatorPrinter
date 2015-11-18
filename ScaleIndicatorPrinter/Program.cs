using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

using ScaleIndicatorPrinter.Models;
using System.IO.Ports;
using Toolbox.NETMF.NET;
using System.Collections;
using Json.NETMF;
using NetduinoRGBLCDShield;
using System.Text;

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

        private static string mRootDirectory { get; set; }

        private static string mLabelFormatFileName { get; set; }

        private static string mJobFileName { get; set; }

        private static string mOperationFileName { get; set; }

        private static string mShopTrakTransactionsURLFileName { get; set; }

        private static string mPieceWeightFileName { get; set; }
        private static double mPieceWeight { get; set; }

        private static string mGrossWeightAdjustmentFileName { get; set; }
        private static double mGrossWeightAdjustment { get; set; }

        private static int mintMenuCount { get { return 5; } } //this represents 5 available menus. Note that the MenuSelection enum has 7 available values but I only want to be able to cycle 
        private static int mintMenuSelection { get; set; }          //through 5 of them since the other 2 AdjustPieceWeight & AdjustGrossWeight are set via the 'Select' Button.
        private static int mMenuSelection { get { return System.Math.Abs(mintMenuSelection); } }

        private static int mintIncrementSelection { get; set; }
        private static int mIncrementSelection { get { return System.Math.Abs(mintIncrementSelection); } }
        private static double[] mIncrements { get; set; }

        private static InterruptPort btnBoard { get; set; }
        private static OutputPort onboardLED = new OutputPort(Pins.ONBOARD_LED, false);

        private static InterruptPort btnShield { get; set; }

        private static CD74HC4067 mMux { get; set; }

        public static void Main()
        {
            //Thread.Sleep(10000);
            try
            {
                ConfigureLCDShield(); //Configure this one first so that errors can be written to the LCD Shield...

                ConfigureMUX();

                ConfigureDefaults();

                RetrieveSettingsFromSDCard();

                ConfigureSerialPorts();

                ConfigureOnBoardButton();

                mintMenuSelection = (int)MenuSelection.PrintLabel;

                DisplayInformation();
            }
            catch (Exception objEx)
            {
                lcdBoard.Clear();
                lcdBoard.SetPosition(0, 0);
                lcdBoard.Write("ERR-" + objEx.Message.Substring(0, 12));
                lcdBoard.SetPosition(1, 0);
                lcdBoard.Write(objEx.Message.Substring(12, 27));
                Debug.Print("Exception caught\r\n");
                Debug.Print(objEx.Message);
            }
            

            // we are done
            Thread.Sleep(Timeout.Infinite);
        }

        public static void ConfigureDefaults()
        {
            mRootDirectory = @"\SD\";
            mLabelFormatFileName = "LabelFormat.txt";
            mJobFileName = "Job.txt";
            mOperationFileName = "Operation.txt";
            mShopTrakTransactionsURLFileName = "ShopTrakTransactionsURL.txt";
            mPieceWeightFileName = "PieceWeight.txt";
            mGrossWeightAdjustmentFileName = "GrossWeightAdjustment.txt";

            mintIncrementSelection = 3;
            mIncrements = new double[] { .001, .01, .1, 1, 10 };
        }

        public static void RetrieveSettingsFromSDCard()
        {

            mSettings = new Settings(new System.IO.DirectoryInfo(mRootDirectory));

            //mSettings.SetLabelFormat(mLabelFormatFileName, Label.SampleLabel);
            mSettings.RetrieveInformationFromFile(mLabelFormatFileName, InformationType.LabelFormat);
            Label.LabelFormat = Settings.LabelFormat;

            //mSettings.SetJobNumber(mJobFileName, "B000053070-0000");
            mSettings.RetrieveInformationFromFile(mJobFileName, InformationType.JobNumber);

            //mSettings.SetOperationNumber(mOperationFileName, "10");
            mSettings.RetrieveInformationFromFile(mOperationFileName, InformationType.OperationNumber);

            //mSettings.SetShopTrakTransactionsURL(mShopTrakTransactionsURLFileName, 
            //    "http://dataservice.wiretechfab.com:6156/SytelineDataService/ShopTrak/LCLTTransaction/Job=~p0&Suffix=~p1&Operation=~p2");
            mSettings.RetrieveInformationFromFile(mShopTrakTransactionsURLFileName, InformationType.ShopTrakTransactionsURL);
            mShopTrakTransactionsURL = Settings.ShopTrakTransactionsURL;

            //mSettings.SetPieceWeight(mPieceWeightFileName, .5);
            mSettings.RetrieveInformationFromFile(mPieceWeightFileName, InformationType.PieceWeight);

            //mSettings.SetGrossWeightAdjustment(mGrossWeightAdjustmentFileName, 10);
            mSettings.RetrieveInformationFromFile(mGrossWeightAdjustmentFileName, InformationType.GrossWeightAdjustment);
            
        }

        public static void ConfigureSerialPorts()
        {
            // initialize the serial port for data being input via COM1 (using D0 & D1) from the 
            mIndicatorScannerSerialPort = new MySerialPort(SerialPorts.COM1, BaudRate.Baudrate9600, Parity.None, DataBits.Eight, StopBits.One);
            
            // initialize the serial port for data being output COM3 (using D7 & D8)
            mPrinterSerialPort = new MySerialPort(SerialPorts.COM3, BaudRate.Baudrate9600, Parity.None, DataBits.Eight, StopBits.One);

            // open the serial-ports, so we can send & receive data
            mIndicatorScannerSerialPort.Open();
            mPrinterSerialPort.Open();

            // add an event-handler for handling incoming data
            mIndicatorScannerSerialPort.DataReceived += new SerialDataReceivedEventHandler(IndicatorScannerSerialPort_DataReceived);
        }

        public static void ConfigureOnBoardButton()
        {
            //InterruptEdgeLevelLow only fires the event the first time that the button descends
            btnBoard = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh);
            // Create an event handler for the button
            btnBoard.OnInterrupt += new NativeEventHandler(btnBoard_OnInterrupt);
        }

        public static void ConfigureLCDShield()
        {
            // the MCP is what allows us to talk with the RGB LCD panel
            mcp23017 = new MCP23017();
            // and this is a class to help us chat with the LCD panel
            lcdBoard = new RGBLCDShield(mcp23017);


            // Setup the interrupt port
            btnShield = new InterruptPort(Pins.GPIO_PIN_D5, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);
            // Bind the interrupt handler to the pin's interrupt event.
            btnShield.OnInterrupt += new NativeEventHandler(btnShield_OnInterrupt);
        }

        public static void ConfigureMUX()
        {
            OutputPort DigitalPin9 = new OutputPort(Pins.GPIO_PIN_D9, false); //Goes to S0
            OutputPort DigitalPin10 = new OutputPort(Pins.GPIO_PIN_D10, false);//Goes to S1
            OutputPort DigitalPin11 = new OutputPort(Pins.GPIO_PIN_D11, false);//Goes to S2
            OutputPort DigitalPin12 = new OutputPort(Pins.GPIO_PIN_D12, false);//Goes to S3

            mMux = new CD74HC4067(DigitalPin9, DigitalPin10, DigitalPin11, DigitalPin12);
            mMux.SetPort(MuxChannel.C0);
        }

        public static void btnShield_OnInterrupt(UInt32 data1, UInt32 data2, DateTime time)
        {
            /*For some reason this event returns data multiple times when a button gets pressed. If you wait a little bit in the debugger, then the variable actually eventually changes to a value that contains a button's value.
             * What I first observed was that a number such as 34304 was returned multiple times before a number such as 34305 would be returned. 34304=0x8600=NoButton, 34305=0x8601=SelectButton (which was indicated as 0x01 in the 
             * Select button Enum). So I initially designed the loop to check for 34304, 34305, 34306, etc. to determine which button was pressed.  However, when I plugged in a different LED Shield, I noticed that the numbers changed
             * I wasn't able to ascertain what those other numbers represented, but I did ascertain that the hex value at the end contained the data of the button that was pressed.  From there I discovered the BitConverter.GetBytes()
             * function that separated the button press number from the other data and so I could use it to grab the button pressed and then use the Button Enumeration to check which button was pressed.
             */
            var ButtonPressed = mcp23017.ReadGpioAB();
            //var ButtonPressed = mcp23017.DigitalRead((byte)ScaleIndicatorPrinter.Models.MCP23017.Command.MCP23017_INTCAPA); //to read the data from a specific pin.

            var InterruptBits = BitConverter.GetBytes(ButtonPressed);
            switch (InterruptBits[0]) //the 0 value contains the button that was pressed...
            {
                case (int)NetduinoRGBLCDShield.Button.Left:
                    if ((mMenuSelection == (int)MenuSelection.AdjustPieceWeight) || (mMenuSelection == (int)MenuSelection.AdjustGrossWeight))
                        mintIncrementSelection = mintIncrementSelection >= mIncrements.Length - 1 ? mintIncrementSelection : ++mintIncrementSelection;
                    else
                    {
                        --mintMenuSelection;
                        DisplayInformation();
                    }
                    break;
                case (int)NetduinoRGBLCDShield.Button.Right:
                    if ((mMenuSelection == (int)MenuSelection.AdjustPieceWeight) || (mMenuSelection == (int)MenuSelection.AdjustGrossWeight))
                        mintIncrementSelection = mintIncrementSelection <= 0 ? mintIncrementSelection : --mintIncrementSelection;
                    else
                    {
                        ++mintMenuSelection;
                        DisplayInformation();
                    }
                    break;
                case (int)NetduinoRGBLCDShield.Button.Up:
                    if (mMenuSelection == (int)MenuSelection.AdjustPieceWeight)
                    {
                        lcdBoard.SetPosition(1, 0);
                        mPieceWeight = mPieceWeight + mIncrements[mIncrementSelection % mIncrements.Length];
                        lcdBoard.Write(mPieceWeight.ToString("F3"));
                    }
                    else if (mMenuSelection == (int)MenuSelection.AdjustGrossWeight)
                    {
                        lcdBoard.SetPosition(1, 0);
                        mGrossWeightAdjustment = mGrossWeightAdjustment + mIncrements[mIncrementSelection % mIncrements.Length];
                        lcdBoard.Write(mGrossWeightAdjustment.ToString("F3"));
                    }
                    break;
                case (int)NetduinoRGBLCDShield.Button.Down:
                    if (mMenuSelection == (int)MenuSelection.AdjustPieceWeight)
                    {
                        lcdBoard.SetPosition(1, 0);
                        mPieceWeight = mPieceWeight - mIncrements[mIncrementSelection % mIncrements.Length];
                        mPieceWeight = mPieceWeight > 0 ? mPieceWeight : 0;
                        lcdBoard.Write(mPieceWeight.ToString("F3"));
                    }
                    else if (mMenuSelection == (int)MenuSelection.AdjustGrossWeight)
                    {
                        lcdBoard.SetPosition(1, 0);
                        mGrossWeightAdjustment = mGrossWeightAdjustment - mIncrements[mIncrementSelection % mIncrements.Length];
                        lcdBoard.Write(mGrossWeightAdjustment.ToString("F3"));
                    }
                    break;
                case (int)NetduinoRGBLCDShield.Button.Select:
                    if (mMenuSelection == (int)MenuSelection.AdjustPieceWeight)
                    {
                        mSettings.SetPieceWeight(mPieceWeightFileName, mPieceWeight);
                        mDataRecieved = RecievedData.None;
                        mintMenuSelection = (int)MenuSelection.ViewPieceWeight;
                        DisplayInformation();
                    }
                    else if (mMenuSelection == (int)MenuSelection.AdjustGrossWeight)
                    {
                        mSettings.SetGrossWeightAdjustment(mGrossWeightAdjustmentFileName, mGrossWeightAdjustment);
                        mDataRecieved = RecievedData.None;
                        mintMenuSelection = (int)MenuSelection.ViewGrossWeightAdjustment;
                        DisplayInformation();
                    }
                    else
                        PerformAction();
                    break;
            }
        }

        private static void btnBoard_OnInterrupt(uint port, uint data, DateTime time)
        {
            lcdBoard.Clear();
            lcdBoard.SetPosition(0, 0);
            lcdBoard.Write("Rebooting...");
            //Makes the LED blink 3 times
            BlinkOnboardLED(3, 300);
            PowerState.RebootDevice(false);

            //For development purposes...
            #region 
            ////Makes the LED blink 3 times
            //BlinkOnboardLED(3, 300);
            ////Fires the Serial Port Data Recieved Event Listener to simulate data being recieved from the serial port. 
            //SerialDataReceivedEventArgs objSerialDataReceivedEventArgs = null;
            //IndicatorScannerSerialPort_DataReceived(new object(), objSerialDataReceivedEventArgs);
            #endregion
        }

        private static void BlinkOnboardLED(int NoOfTimes, int WaitPeriod)
        {
            for (int intCounter = 2; intCounter < NoOfTimes * 2 + 1; intCounter++)
            {
                onboardLED.Write(intCounter % 2 == 1);
                Thread.Sleep(WaitPeriod);
            }
        }

        private static void PerformAction()
        {
            lcdBoard.SetPosition(0, 0);

            switch ((int)mMenuSelection % mintMenuCount)
            {
                case (int)MenuSelection.PrintLabel:
                    mPrinterSerialPort.WriteString(Label.SampleLabel);
                    break;
                case (int)MenuSelection.Job:
                    mDataRecieved = RecievedData.ScannerJobAndSuffix;
                    lcdBoard.Write("Scan Job #...");
                    break;
                case (int)MenuSelection.Operation:
                    mDataRecieved = RecievedData.ScannerOperation;
                    lcdBoard.Write("Scan Op #...");
                    break;
                case (int)MenuSelection.ViewPieceWeight:
                    mintMenuSelection = (int)MenuSelection.AdjustPieceWeight;
                    mDataRecieved = RecievedData.None;
                    mPieceWeight = Settings.PieceWeight;
                    lcdBoard.Write("Adj Pc Weight...");
                    break;
                case (int)MenuSelection.ViewGrossWeightAdjustment:
                    mintMenuSelection = (int)MenuSelection.AdjustGrossWeight;
                    mDataRecieved = RecievedData.None;
                    mGrossWeightAdjustment = Settings.GrossWeightAdjustment;
                    lcdBoard.Write("Adj Gross Weight...");
                    break;
            }
        }

        private static void DisplayInformation()
        {
            lcdBoard.Clear();
            lcdBoard.SetPosition(0, 0);

            mintMenuSelection = mintMenuSelection % mintMenuCount;
            switch (mMenuSelection)
            {
                case (int)MenuSelection.PrintLabel:
                    mDataRecieved = RecievedData.ScaleIndicator;
                    lcdBoard.Write("Press Print...");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write("To print a label");
                    //Tell MUX what channel to listen on...
                    mMux.SetPort(MuxChannel.C0);
                    break;
                case (int)MenuSelection.Job :
                    mDataRecieved = RecievedData.None;
                    lcdBoard.Write("Job:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(Settings.JobNumber);
                    //Tell MUX what channel to listen on...
                    mMux.SetPort(MuxChannel.C1);
                    break;
                case (int)MenuSelection.Operation:
                    mDataRecieved = RecievedData.None;
                    lcdBoard.Write("Operation:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(Settings.Operation.ToString());
                    //Tell MUX what channel to listen on...
                    mMux.SetPort(MuxChannel.C1);
                    break;
                case (int)MenuSelection.ViewPieceWeight:
                    mDataRecieved = RecievedData.None;
                    lcdBoard.Write("Piece Weight:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(Settings.PieceWeight.ToString("F3"));
                    break;
                case (int)MenuSelection.ViewGrossWeightAdjustment:
                    mDataRecieved = RecievedData.None;
                    lcdBoard.Write("Gross Weight Adjustment:");
                    lcdBoard.SetPosition(1, 0);
                    lcdBoard.Write(Settings.GrossWeightAdjustment.ToString("F3"));
                    break;
            }
        }

        private static void IndicatorScannerSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var strMessage = mIndicatorScannerSerialPort.ReadString();
            if (strMessage == string.Empty || strMessage == null )
                return;

            switch (mDataRecieved)
            {
                case RecievedData.ScaleIndicator:
                    var objIndicatorData = new IndicatorData(strMessage);

                    if (objIndicatorData.HasValidDataString)
                    {
                        /*A new thread must be started in order for the WebGet function to work properly; otherwise WebGet(objIndicatorData) would just silently fail...
                         * http://www.codeproject.com/Articles/795829/Multithreading-with-Netduino-and-NET-Microframework
                         * https://www.youtube.com/watch?v=YZOrORB88-s */
                        var DataRequestThread = new Thread(delegate() { WebGet(objIndicatorData); });
                        DataRequestThread.Start();
                    }
                    break;
                case RecievedData.ScannerJobAndSuffix:
                    mSettings.SetJobNumber(mJobFileName, strMessage);
                    DisplayInformation();
                    break;
                case RecievedData.ScannerOperation:
                    mSettings.SetOperationNumber(mOperationFileName, strMessage);
                    DisplayInformation();
                    break;
            }

            
        }

        public static void WebGet(IndicatorData objIndicatorData)
        {
            var URL = Settings.ShopTrakTransactionsURL.SetParameters(new string[] { Settings.Job, Settings.Suffix.ToString(), Settings.Operation.ToString() });
            //var URL = @"http://10.1.0.55:6156/SytelineDataService/ShopTrak/LCLTTransaction/Job=B000053094&Suffix=0&Operation=10"; //localhost URL
            //var URL = @"http://dataservice.wiretechfab.com:3306/SytelineDataService/ShopTrak/LCLTTransaction/Job=B000053094&Suffix=0&Operation=10"; //external URL
            //var URL = @"http://dataservice.wiretechfab.com:6156/SytelineDataService/ShopTrak/LCLTTransaction/Job=B000053089&Suffix=0&Operation=10"; //internal URL


            var objURI = new Uri(URL);
            
            var webClient = new HTTP_Client(new IntegratedSocket(objURI.Host, (ushort)objURI.Port));
            var response = webClient.Get(objURI.AbsolutePath);

            // Did we get the expected response? (a "200 OK")
            if (response.ResponseCode != 200)
            {
                Debug.Print("Unexpected HTTP response code: " + response.ResponseCode.ToString());
                lcdBoard.Clear();
                lcdBoard.SetPosition(0, 1);
                lcdBoard.Write("Unexpected HTTP Resp");
                lcdBoard.SetPosition(1, 0);
                lcdBoard.Write(response.ResponseCode.ToString());
                throw new ApplicationException("Unexpected HTTP response code: " + response.ResponseCode.ToString());
            }
            else if (response.ResponseBody == "[]") //Does the REST Dataset return empty?
            {
                Debug.Print("Nobody is punched into that job");
                lcdBoard.Clear();
                lcdBoard.SetPosition(0, 1);
                lcdBoard.Write("Nobody is");
                lcdBoard.SetPosition(1, 0);
                lcdBoard.Write("Punched in...");
                throw new ApplicationException("Nobody is punched into that Job...");
            }
 
            ArrayList arrayList = JsonSerializer.DeserializeString(response.ResponseBody) as ArrayList;
            Hashtable hashtable = arrayList[0] as Hashtable; //get the first row of records

            //Microsoft.SPOT.Time.TimeService.SetTimeZoneOffset(300);
            var CurrentDateTime = DateTimeExtensions.FromASPNetAjax(hashtable["CurrentDateTime"].ToString()).AddHours(-5);//Central Time Zone has 5 hour offset from UTC
            var Item = hashtable["item"].ToString();

            StringBuilder strBldrEmployees = new StringBuilder();
            for (int intCounter = 0; intCounter < arrayList.Count; intCounter++) //iterate over all the rows to get the employees that are punched into the jobs
            {
                hashtable = arrayList[intCounter] as Hashtable;
                strBldrEmployees.Append(hashtable["emp_num"].ToString().Trim() + ",");
            }
            strBldrEmployees.Remove(strBldrEmployees.ToString().LastIndexOf(","), 1); //remove the last comma from the string

            var Pieces = (objIndicatorData.NetWeight + mGrossWeightAdjustment) / Settings.PieceWeight;
            var objLabel = new Label(new string[] { Item, Settings.JobNumber, Settings.Operation.ToString("D3"), strBldrEmployees.ToString(), ((int)Pieces).ToString(), CurrentDateTime.ToString("MM/dd/yy h:mm:ss tt"), CurrentDateTime.ToString("dddd") });
            mPrinterSerialPort.WriteString(objLabel.LabelText);

        }
    }
}
