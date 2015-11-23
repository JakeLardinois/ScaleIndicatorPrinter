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
        
        private static Settings mSettings { get; set; }
        public static Menu mMenu { get; set; }

        private static InterruptPort btnBoard { get; set; }
        private static OutputPort onboardLED = new OutputPort(Pins.ONBOARD_LED, false);
        private static InterruptPort btnShield { get; set; }
        

        public static void Main()
        {
            try
            {
                mMenu = new Menu();//Configure this one first so that errors can be written to the LCD Shield...
                mSettings = new Settings();

                // Setup the interrupt port for button presses from the LCD Shield
                btnShield = new InterruptPort(Pins.GPIO_PIN_D5, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);
                // Bind the interrupt handler to the pin's interrupt event.
                btnShield.OnInterrupt += new NativeEventHandler(btnShield_OnInterrupt);

                mMenu.Increments = new double[] { .001, .01, .1, 1, 10 };
                mMenu.IncrementSelection = 3;
                mMenu.MenuSelection = MenuSelection.PrintLabel;
                mMenu.Mux.SetPort(MuxChannel.C0);

                mSettings.RootDirectoryPath = @"\SD\";
                mSettings.LabelFormatFileName = "LabelFormat.txt";
                mSettings.JobNumberFileName = "Job.txt";
                mSettings.OperationFileName = "Operation.txt";
                mSettings.ShopTrakTransactionsURLFileName = "ShopTrakTransactionsURL.txt";
                mSettings.PieceWeightFileName = "PieceWeight.txt";
                mSettings.NetWeightAdjustmentFileName = "NetWeightAdjustment.txt";
                mSettings.RetrieveSettingsFromSDCard();

                // initialize the serial port for data being input via COM1
                mIndicatorScannerSerialPort = new MySerialPort(SerialPorts.COM1, BaudRate.Baudrate9600, Parity.None, DataBits.Eight, StopBits.One);
                // initialize the serial port for data being output via COM3
                mPrinterSerialPort = new MySerialPort(SerialPorts.COM3, BaudRate.Baudrate9600, Parity.None, DataBits.Eight, StopBits.One);
                // open the serial-ports, so we can send & receive data
                mIndicatorScannerSerialPort.Open();
                mPrinterSerialPort.Open();
                // add an event-handler for handling incoming data
                mIndicatorScannerSerialPort.DataReceived += new SerialDataReceivedEventHandler(IndicatorScannerSerialPort_DataReceived);


                //Setup the Onboard button; InterruptEdgeLevelLow only fires the event the first time that the button descends
                btnBoard = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh);
                // Create an event handler for the button
                btnBoard.OnInterrupt += new NativeEventHandler(btnBoard_OnInterrupt);

                //Display appropriarate information to the user...
                mMenu.DisplayInformation(mSettings);
            }
            catch (Exception objEx)
            {
                Debug.Print("Exception caught\r\n");
                Debug.Print(objEx.Message);
                mMenu.DisplayError(objEx);
            }

            // we are done
            Thread.Sleep(Timeout.Infinite);
        }

        public static void btnShield_OnInterrupt(UInt32 data1, UInt32 data2, DateTime time)
        {
            /*For some reason this event returns data multiple times when a button gets pressed. If you wait a little bit in the debugger, then the variable actually eventually changes to a value that contains a button's value.
             * What I first observed was that a number such as 34304 was returned multiple times before a number such as 34305 would be returned. 34304=0x8600=NoButton, 34305=0x8601=SelectButton (which was indicated as 0x01 in the 
             * Select button Enum). So I initially designed the loop to check for 34304, 34305, 34306, etc. to determine which button was pressed.  However, when I plugged in a different LED Shield, I noticed that the numbers changed
             * I wasn't able to ascertain what those other numbers represented, but I did ascertain that the hex value at the end contained the data of the button that was pressed.  From there I discovered the BitConverter.GetBytes()
             * function that separated the button press number from the other data and so I could use it to grab the button pressed and then use the Button Enumeration to check which button was pressed.
             */
            var ButtonPressed = mMenu.mcp23017.ReadGpioAB();
            //var ButtonPressed = mcp23017.DigitalRead((byte)ScaleIndicatorPrinter.Models.MCP23017.Command.MCP23017_INTCAPA); //to read the data from a specific pin.

            var InterruptBits = BitConverter.GetBytes(ButtonPressed);
            switch (InterruptBits[0]) //the 0 value contains the button that was pressed...
            {
                case (int)NetduinoRGBLCDShield.Button.Left:
                    if ((mMenu.MenuSelection == MenuSelection.AdjustPieceWeight) || (mMenu.MenuSelection == MenuSelection.AdjustNetWeight))
                        mMenu.IncrementSelection = mMenu.IncrementSelection >= mMenu.Increments.Length - 1 ? mMenu.IncrementSelection : ++mMenu.IncrementSelection;
                    else
                    {
                        --mMenu.intMenuSelection;
                        mMenu.DisplayInformation(mSettings);
                    }
                    break;
                case (int)NetduinoRGBLCDShield.Button.Right:
                    if ((mMenu.MenuSelection == MenuSelection.AdjustPieceWeight) || (mMenu.MenuSelection == MenuSelection.AdjustNetWeight))
                        mMenu.IncrementSelection = mMenu.IncrementSelection <= 0 ? mMenu.IncrementSelection : --mMenu.IncrementSelection;
                    else
                    {
                        ++mMenu.intMenuSelection;
                        mMenu.DisplayInformation(mSettings);
                    }
                    break;
                case (int)NetduinoRGBLCDShield.Button.Up:
                    if (mMenu.MenuSelection == MenuSelection.AdjustPieceWeight)
                    {
                        mSettings.PieceWeight = mSettings.PieceWeight + mMenu.Increments[mMenu.IncrementSelection];
                        mMenu.DisplayPieceWeight(mSettings);
                    }
                    else if (mMenu.MenuSelection == MenuSelection.AdjustNetWeight)
                    {
                        mSettings.NetWeightAdjustment = mSettings.NetWeightAdjustment + mMenu.Increments[mMenu.IncrementSelection];
                        mMenu.DisplayNetWeightAdjustment(mSettings);
                    }
                    break;
                case (int)NetduinoRGBLCDShield.Button.Down:
                    if (mMenu.MenuSelection == MenuSelection.AdjustPieceWeight)
                    {
                        mSettings.PieceWeight = mSettings.PieceWeight - mMenu.Increments[mMenu.IncrementSelection];
                        mMenu.DisplayPieceWeight(mSettings);
                    }
                    else if (mMenu.MenuSelection == MenuSelection.AdjustNetWeight)
                    {
                        mSettings.NetWeightAdjustment = mSettings.NetWeightAdjustment - mMenu.Increments[mMenu.IncrementSelection];
                        mMenu.DisplayNetWeightAdjustment(mSettings);
                    }
                    break;
                case (int)NetduinoRGBLCDShield.Button.Select:
                    if (mMenu.MenuSelection == MenuSelection.AdjustPieceWeight)
                    {
                        mSettings.StorePieceWeight();
                        mMenu.DataRecieved = RecievedData.None;
                        mMenu.MenuSelection = MenuSelection.ViewPieceWeight;
                        mMenu.DisplayInformation(mSettings);
                    }
                    else if (mMenu.MenuSelection == MenuSelection.AdjustNetWeight)
                    {
                        mSettings.StoreNetWeightAdjustment();
                        mMenu.DataRecieved = RecievedData.None;
                        mMenu.MenuSelection = MenuSelection.ViewNetWeightAdjustment;
                        mMenu.DisplayInformation(mSettings);
                    }
                    else
                        PerformAction();
                    break;
            }
        }

        private static void btnBoard_OnInterrupt(uint port, uint data, DateTime time)
        {
            mMenu.lcdBoard.Clear();
            mMenu.lcdBoard.SetPosition(0, 0);
            mMenu.lcdBoard.Write("Rebooting...");
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
            mMenu.lcdBoard.SetPosition(0, 0);

            switch (mMenu.AvailableMenuSelection)
            {
                case MenuSelection.PrintLabel:
                    mPrinterSerialPort.WriteString(Label.DefaultLabel);
                    break;
                case MenuSelection.Job:
                    mMenu.DataRecieved = RecievedData.ScannerJobAndSuffix;
                    mMenu.lcdBoard.Write("Scan Job #...");
                    break;
                case MenuSelection.Operation:
                    mMenu.DataRecieved = RecievedData.ScannerOperation;
                    mMenu.lcdBoard.Write("Scan Op #...");
                    break;
                case MenuSelection.ViewPieceWeight:
                    mMenu.MenuSelection = MenuSelection.AdjustPieceWeight;
                    mMenu.DataRecieved = RecievedData.None;
                    mMenu.lcdBoard.Write("Adj Pc Weight...");
                    break;
                case MenuSelection.ViewNetWeightAdjustment:
                    mMenu.MenuSelection = MenuSelection.AdjustNetWeight;
                    mMenu.DataRecieved = RecievedData.None;
                    mMenu.lcdBoard.Write("Adj Net Weight...");
                    break;
            }
        }

        private static void IndicatorScannerSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var strMessage = mIndicatorScannerSerialPort.ReadString();
            if (strMessage == string.Empty || strMessage == null )
                return;

            switch (mMenu.DataRecieved)
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
                    mSettings.JobNumber = strMessage;
                    mSettings.StoreJobNumber();
                    mMenu.DisplayInformation(mSettings);
                    break;
                case RecievedData.ScannerOperation:
                    mSettings.Operation = strMessage;
                    mSettings.StoreOperationNumber();
                    mMenu.DisplayInformation(mSettings);
                    break;
            }
            
        }

        public static void WebGet(IndicatorData objIndicatorData)
        {
            try
            {
                var URL = mSettings.ShopTrakTransactionsURL.SetParameters(new string[] { mSettings.Job, mSettings.Suffix.ToString(), mSettings.Operation.ToString() });
                var objURI = new Uri(URL);
                var webClient = new HTTP_Client(new IntegratedSocket(objURI.Host, (ushort)objURI.Port));
                var response = webClient.Get(objURI.AbsolutePath);


                if (response.ResponseCode != 200) // Did we get the expected response? (a "200 OK")
                    throw new ApplicationException("HTTP Response: " + response.ResponseCode.ToString());
                else if (response.ResponseBody == "[]") //Does the REST Dataset return empty?
                    throw new ApplicationException("Nobody punched in that Job.");
 
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

                var Pieces = (objIndicatorData.NetWeight + mSettings.NetWeightAdjustment) / mSettings.PieceWeight;
                var objLabel = new Label(new string[] { Item, mSettings.JobNumber, mSettings.OperationNumber.ToString("D3"), strBldrEmployees.ToString(), ((int)Pieces).ToString(), CurrentDateTime.ToString("MM/dd/yy h:mm:ss tt"), CurrentDateTime.ToString("dddd") });
                mPrinterSerialPort.WriteString(objLabel.LabelText);
            }
            catch (Exception objEx)
            {
                mMenu.DisplayError(objEx);
            }
        }

    }
}
