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
using Rinsen.WebServer.FileAndDirectoryServer;



namespace ScaleIndicatorPrinter
{
    public class Program
    {
        private static MySerialPort mIndicatorScannerSerialPort;
        private static MySerialPort mPrinterSerialPort;

        private static CD74HC4067 Mux { get; set; }
        private static MCP23017 mcp23017 { get; set; }

        public static Settings Settings { get; set; }
        private static Menu mMenu { get; set; }

        private static InterruptPort btnBoard { get; set; }
        private static OutputPort onboardLED = new OutputPort(Pins.ONBOARD_LED, false);
        private static InterruptPort btnShield { get; set; }
        

        public static void Main()
        {
            try
            {
                mcp23017 = new MCP23017();//the MCP is what allows us to talk with the RGB LCD panel. I need it in this class so I can read the button presses from the User...
                mMenu = new Menu(mcp23017);//Configure this one first so that errors can be written to the LCD Shield
                mMenu.MenuSelection = MenuSelection.PrintLabel;

                //Setup the interrupt port for button presses from the LCD Shield. 
                //Here I have the Interrupt pin from the LCD Shield (configured in the MCP23017 class) going to the Netduino Digital Pin 5
                btnShield = new InterruptPort(Pins.GPIO_PIN_D5, true, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);
                // Bind the interrupt handler to the pin's interrupt event.
                btnShield.OnInterrupt += new NativeEventHandler(btnShield_OnInterrupt);

                //Configure the MUX which allows me to expand my serial ports. Here I am using digital pins 9 thru 10 to send the necessary signals to switch my MUX channels
                OutputPort DigitalPin9 = new OutputPort(Pins.GPIO_PIN_D9, false); //Goes to S0
                OutputPort DigitalPin10 = new OutputPort(Pins.GPIO_PIN_D10, false);//Goes to S1
                OutputPort DigitalPin11 = new OutputPort(Pins.GPIO_PIN_D11, false);//Goes to S2
                OutputPort DigitalPin12 = new OutputPort(Pins.GPIO_PIN_D12, false);//Goes to S3
                Mux = new CD74HC4067(DigitalPin9, DigitalPin10, DigitalPin11, DigitalPin12);
                Mux.SetPort(MuxChannel.C0); //default it to C0 which is data coming in from the Indicators Serial Port

                Settings = new Settings();
                Settings.Increments = new double[] { .001, .01, .1, 1, 10, 100 };
                Settings.IncrementSelection = 3;
                Settings.RetrieveSettingsFromSDCard(@"\WWW\", "LabelFormat.txt", "Job.txt", "Operation.txt", "ShopTrakTransactionsURL.txt",
                    "PieceWeight.txt", "NetWeightAdjustment.txt", "BackgroundColor.txt");

                mMenu.SetBackLightColor(Settings.BacklightColor);

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

                // write your code here
                var webServer = new WebServer();
                webServer.AddRequestFilter(new RequestFilter());
                webServer.SetFileAndDirectoryService(new FileAndDirectoryService());
                webServer.RouteTable.DefaultControllerName = "Default";
                webServer.StartServer(80);//If port is not specified, then default is port 8500

                //Display appropriate information to the user...
                mMenu.DisplayInformation(Settings);
            }
            catch (Exception objEx)
            {
                Debug.Print("Exception caught in Main()\r\n");
                Debug.Print(objEx.Message);
                mMenu.DisplayError(objEx);
            }

            //We are done. The thread must sleep or else the netduino turns off...
            Thread.Sleep(Timeout.Infinite);
        }

        public static void btnShield_OnInterrupt(UInt32 data1, UInt32 data2, DateTime time)
        {

            try
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

                /*the 0 value contains the button that was pressed... I had an issue with this when I coded the functionality to change the background display because as soon as I changed the Background color from the default of White (which 
                 * was set in the RGBLCDShield class constructor) my buttons on the LCD shield would stop working. I discovered that only the first 5 bits of the InterruptBits[0] byte contained the button that was pressed.  What was happening was
                 * that when I changed the LCD backlight color, additional bits were being added to the front of the byte and so the integer that was cast with (NetduinoRGBLCDShield.Button)InterruptBits[0] in my switch statement did not recognize
                 * a button press...  I observed this as my UI stopping working and had a LOT of difficulty troubleshooting! I added the below default to the Switch statement so that future debugging sessions would verify that my button events were
                 * firing, but I will still recieve a lot of false posotives since a single button press will also send empty bytes... At first I tried to shift the bits using (NetduinoRGBLCDShield.Button)(InterruptBits[0] >> 3)
                 * which would shift the left (if InterruptBits[0] was 1000, the statement would yield 62) but realized that this wasn't the proper method since I was still keeping the higher order bits. The solution was to perform a bitwise AND operation (&) on the values using a mask
                 * that masked out the pertinant bits (in this case 31 or 0x1F or 11111) so that if InterruptBits[0] was set at 1,000 (1111101000 in binary) then an AND against 31 (11111 binary) would yield 8 (1111101000 AND 0000011111 = 1000)
                 */
                var ButtonValue = (InterruptBits[0] & 0x1F);
                if (ButtonValue == 0)
                    return;
                Debug.Print("Pressed Button Value is: " + ButtonValue);
                Debug.Print("Menu selection is: " + mMenu.MenuSelection);
                switch ((NetduinoRGBLCDShield.Button)ButtonValue)
                {
                    case NetduinoRGBLCDShield.Button.Left:
                        if ((mMenu.MenuSelection == MenuSelection.AdjustPieceWeight) || (mMenu.MenuSelection == MenuSelection.AdjustNetWeight))
                            ++Settings.IncrementSelection;
                        else if (mMenu.MenuSelection == MenuSelection.ChangeBackLightColor)
                        {
                            Settings.NextBacklightColor();
                            mMenu.SetBackLightColor(Settings.BacklightColor);
                            mMenu.DisplayInformation(Settings);
                        }
                        else
                        {
                            mMenu.GoToPreviousAvailableMenuSelection();
                            mMenu.DisplayInformation(Settings);
                        }
                        break;
                    case NetduinoRGBLCDShield.Button.Right:
                        if ((mMenu.MenuSelection == MenuSelection.AdjustPieceWeight) || (mMenu.MenuSelection == MenuSelection.AdjustNetWeight))
                            --Settings.IncrementSelection;
                        else if (mMenu.MenuSelection == MenuSelection.ChangeBackLightColor)
                        {
                            Settings.PreviousBacklightColor();
                            mMenu.SetBackLightColor(Settings.BacklightColor);
                            mMenu.DisplayInformation(Settings);
                        }
                        else
                        {
                            mMenu.GoToNextAvailableMenuSelection();
                            mMenu.DisplayInformation(Settings);
                        }
                        break;
                    case NetduinoRGBLCDShield.Button.Up:
                        if (mMenu.MenuSelection == MenuSelection.AdjustPieceWeight)
                            Settings.IncrementPieceWeight();
                        else if (mMenu.MenuSelection == MenuSelection.AdjustNetWeight)
                            Settings.IncrementNetWeightAdjustment();
                        mMenu.DisplayInformation(Settings);
                        break;
                    case NetduinoRGBLCDShield.Button.Down:
                        if (mMenu.MenuSelection == MenuSelection.AdjustPieceWeight)
                            Settings.DecrementPieceWeight();
                        else if (mMenu.MenuSelection == MenuSelection.AdjustNetWeight)
                            Settings.DecrementNetWeightAdjustment();
                        mMenu.DisplayInformation(Settings);
                        break;
                    case NetduinoRGBLCDShield.Button.Select:
                        if (mMenu.MenuSelection == MenuSelection.AdjustPieceWeight)
                        {
                            Settings.StorePieceWeight();
                            mMenu.MenuSelection = MenuSelection.ViewPieceWeight;
                            mMenu.DisplayInformation(Settings);
                        }
                        else if (mMenu.MenuSelection == MenuSelection.AdjustNetWeight)
                        {
                            Settings.StoreNetWeightAdjustment();
                            mMenu.MenuSelection = MenuSelection.ViewNetWeightAdjustment;
                            mMenu.DisplayInformation(Settings);
                        }
                        else if (mMenu.MenuSelection == MenuSelection.ChangeBackLightColor)
                        {
                            Settings.StoreBacklightColor();
                            mMenu.SetBackLightColor(Settings.BacklightColor);
                            mMenu.MenuSelection = MenuSelection.ViewBackLightColor;
                            mMenu.DisplayInformation(Settings);
                        }
                        else
                            PerformAction();
                        break;
                    default:
                        Debug.Print("Unrecognized Button...");
                        throw new Exception("Unrecognized Button Pressed");
                }
            }
            catch(Exception objEx)
            {
                Debug.Print("Exception caught in btnShield_OnInterrupt()\r\n");
                Debug.Print(objEx.Message);
                mMenu.DisplayError(objEx);
            }
        }

        private static void btnBoard_OnInterrupt(uint port, uint data, DateTime time)
        {
            mMenu.MenuSelection = MenuSelection.Reboot;
            PerformAction();

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
            Debug.Print("Blinking LED");
            for (int intCounter = 2; intCounter < NoOfTimes * 2 + 1; intCounter++)
            {
                onboardLED.Write(intCounter % 2 == 1);
                Thread.Sleep(WaitPeriod);
            }
        }

        private static void PerformAction()
        {
            switch (mMenu.MenuSelection)
            {
                case MenuSelection.PrintLabel:
                    //Tell MUX what channel to write to...
                    Debug.Print("Set Mux to Channel 0...");
                    Mux.SetPort(MuxChannel.C0);
                    Debug.Print("Write Default label to Serial Port...");
                    mPrinterSerialPort.WriteString(Label.DefaultLabel);
                    break;
                case MenuSelection.Job:
                    mMenu.DataRecieved = RecievedData.ScannerJobAndSuffix;
                    //Tell MUX what channel to listen on...
                    Debug.Print("Set Mux to Channel 1...");
                    Mux.SetPort(MuxChannel.C1);
                    Debug.Print("Wait for Job Number from Scanner...");
                    break;
                case MenuSelection.Operation:
                    mMenu.DataRecieved = RecievedData.ScannerOperation;
                    //Tell MUX what channel to listen on...
                    Debug.Print("Set Mux to Channel 1...");
                    Mux.SetPort(MuxChannel.C1);
                    Debug.Print("Wait for Operation from Scanner...");
                    break;
                case MenuSelection.ViewPieceWeight:
                    mMenu.DataRecieved = RecievedData.None;
                    Debug.Print("Set Menu to Adjust Piece Weight...");
                    mMenu.MenuSelection = MenuSelection.AdjustPieceWeight;
                    break;
                case MenuSelection.ViewNetWeightAdjustment:
                    mMenu.DataRecieved = RecievedData.None;
                    Debug.Print("Set Menu to Adjust Net Weight...");
                    mMenu.MenuSelection = MenuSelection.AdjustNetWeight;
                    break;
                case MenuSelection.ViewBackLightColor:
                    mMenu.DataRecieved = RecievedData.None;
                    Debug.Print("Set Menu to Adjust Background Color...");
                    mMenu.MenuSelection = MenuSelection.ChangeBackLightColor;
                    break;
                case MenuSelection.ViewNetworkInfo:
                    mMenu.DataRecieved = RecievedData.None;
                    Debug.Print("Set Menu to Display Network Info...");
                    mMenu.MenuSelection = MenuSelection.DisplayNetworkInfo;
                    Settings.RetrieveNetworkSettings(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0]);
                    break;
                case MenuSelection.Reboot:
                    mMenu.MenuSelection = MenuSelection.Rebooting;
                    mMenu.DisplayInformation(Settings);
                    BlinkOnboardLED(3, 300);
                    Debug.Print("Rebooting...");
                    PowerState.RebootDevice(false);
                    break;
            }
            mMenu.DisplayInformation(Settings);
        }

        private static void IndicatorScannerSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var strMessage = mIndicatorScannerSerialPort.ReadString();
            if (strMessage == string.Empty || strMessage == null )
                return;

            Debug.Print("The type of data expected is: " + mMenu.DataRecieved.GetName());
            Debug.Print("Data contents recieved from the Serial Port:\r\n" + strMessage);
            switch (mMenu.DataRecieved)
            {
                case RecievedData.ScaleIndicator:
                    var objIndicatorData = new IndicatorData(strMessage);

                        if (objIndicatorData.HasValidDataString)
                        {
                            Debug.Print("Valid Data was sent from the Indicator...");
                            Settings.NetWeight = objIndicatorData.NetWeight;
                            if (mMenu.MenuSelection == MenuSelection.ViewPieceCount)
                                mMenu.DisplayInformation(Settings);
                            else
                            {
                                /*A new thread must be started in order for the WebGet function to work properly; otherwise WebGet(objIndicatorData) would just silently fail...
                             * http://www.codeproject.com/Articles/795829/Multithreading-with-Netduino-and-NET-Microframework
                             * https://www.youtube.com/watch?v=YZOrORB88-s */
                                var DataRequestThread = new Thread(delegate() { WebGet(objIndicatorData); });
                                DataRequestThread.Start();
                            }
                            
                        }
                    
                   
                    break;
                case RecievedData.ScannerJobAndSuffix:
                    Settings.JobNumber = strMessage;
                    Settings.StoreJobNumber();
                    mMenu.DataRecieved = RecievedData.None;
                    mMenu.DisplayInformation(Settings);
                    break;
                case RecievedData.ScannerOperation:
                    Settings.Operation = strMessage;
                    Settings.StoreOperationNumber();
                    mMenu.DataRecieved = RecievedData.None;
                    mMenu.DisplayInformation(Settings);
                    break;
            }
            
        }

        public static void WebGet(IndicatorData objIndicatorData)
        {
            try
            {
                var URL = Settings.ShopTrakTransactionsURL.SetParameters(new string[] { Settings.Job, Settings.Suffix.ToString(), Settings.Operation.ToString() });
                Debug.Print("WebGet URL is: " + URL);
                var objURI = new Uri(URL);
                var webClient = new HTTP_Client(new IntegratedSocket(objURI.Host, (ushort)objURI.Port));
                var response = webClient.Get(objURI.AbsolutePath);

                Debug.Print("Data recieved from URL is:\r\n" + response.ResponseBody);
                if (response.ResponseCode != 200) // Did we get the expected response? (a "200 OK")
                    throw new ApplicationException("HTTP Response: " + response.ResponseCode.ToString());
                else if (response.ResponseBody == "[]") //Does the REST Dataset return empty?
                    throw new ApplicationException("Nobody punched in that Job.");


                ArrayList arrayList = JsonSerializer.DeserializeString(response.ResponseBody) as ArrayList;
                Hashtable hashtable = arrayList[0] as Hashtable; //get the first row of records

                //Microsoft.SPOT.Time.TimeService.SetTimeZoneOffset(300);
                Settings.PrintDateTime = DateTimeExtensions.FromASPNetAjax(hashtable["CurrentDateTime"].ToString()).AddHours(-5);//Central Time Zone has 5 hour offset from UTC
                Settings.Item = hashtable["item"].ToString();

                StringBuilder strBldrEmployees = new StringBuilder();
                for (int intCounter = 0; intCounter < arrayList.Count; intCounter++) //iterate over all the rows to get the employees that are punched into the jobs
                {
                    hashtable = arrayList[intCounter] as Hashtable;
                    strBldrEmployees.Append(hashtable["emp_num"].ToString().Trim() + ",");
                }
                strBldrEmployees.Remove(strBldrEmployees.ToString().LastIndexOf(","), 1); //remove the last comma from the string
                Settings.Employees = strBldrEmployees.ToString();
                

                //Instantiate my label so that I can populate the Format property with the value pulled from the SDCard.
                var objLabel = new Label(new string[] { Settings.Item, Settings.JobNumber, Settings.OperationNumber.ToString("D3"), Settings.Employees, 
                    Settings.PieceCount.ToString("N"), Settings.PrintDateTime.ToString("MM/dd/yy h:mm:ss tt"), Settings.PrintDateTime.ToString("dddd") });
                objLabel.LabelFormat = Settings.LabelFormat;
                Debug.Print("Data written to printer serial port is:\r\n" + objLabel.LabelText);
                mPrinterSerialPort.WriteString(objLabel.LabelText);
            }
            catch (Exception objEx)
            {
                Debug.Print("Exception caught in WebGet()\r\n");
                Debug.Print(objEx.Message);
                mMenu.DisplayError(objEx);
            }
        }

    }
}
