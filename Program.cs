using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

using System.IO.Ports;
using System.Text;
using Toolbox.NETMF;

namespace ScaleIndicatorPrinter
{
    public class Program
    {
        static SerialPort IndicatorSerialPort;
        static SerialPort PrinterSerialPort;


        public static void Main()
        {
            // initialize the serial port for COM1 (using D0 & D1) and COM2 (using D2 & D3)
            IndicatorSerialPort = new SerialPort(SerialPorts.COM1, 9600, Parity.None, 8, StopBits.One);
            PrinterSerialPort = new SerialPort(SerialPorts.COM2, 9600, Parity.None, 8, StopBits.One);

            // open the serial-ports, so we can send & receive data
            IndicatorSerialPort.Open();
            PrinterSerialPort.Open();

            // add an event-handler for handling incoming data
            IndicatorSerialPort.DataReceived += new SerialDataReceivedEventHandler(IndicatorSerialPort_DataReceived);
            //PrinterSerialPort.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);

            // wait forever...
            Thread.Sleep(Timeout.Infinite);

        }

        static void IndicatorSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] bytes;
            StringBuilder objStrBldr = new StringBuilder();

            // wait a little for the buffer to fill
            System.Threading.Thread.Sleep(100);

            while (IndicatorSerialPort.BytesToRead > 0)
            {
                // create an array for the incoming bytes
                bytes = new byte[IndicatorSerialPort.BytesToRead];
                // read the bytes
                IndicatorSerialPort.Read(bytes, 0, bytes.Length);
                // convert the bytes into a string
                objStrBldr.Append(Tools.Bytes2Chars(bytes));
            }

            if (objStrBldr.Length > 0)//When the stringbuilder was empty, an error was thrown by ToCharArray()
            {
                //var objLabel = new Label(new string[] { "HelloLabel", "HelloBarcode" });
                var objParameters = objStrBldr.ToString().Split(new char[] { '\r' });
                var objLabel = new Label(new string[] { objParameters[0], objParameters[1] });

                //var newByteArray = Tools.Chars2Bytes(objStrBldr.ToString().ToCharArray());
                var newByteArray = Tools.Chars2Bytes(objLabel.LabelText.ToCharArray());
                objStrBldr.Clear();
                //IndicatorSerialPort.Write(newByteArray, 0, newByteArray.Length);
                PrinterSerialPort.Write(newByteArray, 0, newByteArray.Length);
            }

            
        }

    }
}
