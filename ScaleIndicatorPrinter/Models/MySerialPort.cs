using System;
using Microsoft.SPOT;

using System.IO.Ports;
using System.Text;
using Toolbox.NETMF;


namespace ScaleIndicatorPrinter.Models
{
    public class MySerialPort : SerialPort
    {
        public MySerialPort(string PortName, BaudRate BaudRate, Parity Parity, DataBits DataBits, StopBits StopBits)
            : base(PortName, (int)BaudRate, Parity, (int)DataBits, StopBits) {}

        public string ReadString()
        {
            byte[] bytes;
            StringBuilder objStrBldr = new StringBuilder();
            

            //Reads all the data sent from the Indicator or Scanner and converts it into text
            while (this.BytesToRead > 0)
            {
                // create an array for the incoming bytes
                bytes = new byte[this.BytesToRead];
                // read the bytes
                this.Read(bytes, 0, bytes.Length);
                // convert the bytes into a string
                objStrBldr.Append(Tools.Bytes2Chars(bytes));

                // wait a little for the buffer to fill
                System.Threading.Thread.Sleep(50);
            }

            return objStrBldr.Length > 0 ? objStrBldr.ToString().Trim() : string.Empty;
                
        }

        public void WriteString(string strMessage)
        {
            var newByteArray = Tools.Chars2Bytes(strMessage.ToCharArray());
            this.Write(newByteArray, 0, newByteArray.Length);
        }

    }
}
