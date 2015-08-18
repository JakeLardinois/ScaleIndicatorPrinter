using Microsoft.SPOT.Hardware;
using System;

namespace NetduinoRGBLCDShield
{
    public class NetduinoI2CDevice
    {
        // Note:  Running the i2c at 400khz or faster gives problems on some
        // adafruit boards.  This may be due to the type of pullups they used,
        // or an issue with the netduino firmware.  I'm not too sure, but
        // 200kHz seems to fix the issue and I haven't been able to reliably debug it.
        private const int DefaultClockRate = 200;
        private const int TransactionTimeout = 50;

        private I2CDevice.Configuration i2cConfig;
        private static I2CDevice i2cDevice;

        public byte Address { get; private set; }

        // Statically allocate some byte arrays to reduce GC pressure
        // We are on embedded after all ;)
        private byte[] size1 = new byte[1];
        private byte[] size2 = new byte[2];
        private byte[] readBuffer = new byte[1];
        private I2CDevice.I2CTransaction[] readTransaction = new I2CDevice.I2CTransaction[1];
        private I2CDevice.I2CTransaction[] writeTransaction = new I2CDevice.I2CTransaction[1];

        public NetduinoI2CDevice(byte address, int clockRateKhz)
        {
            this.Address = address;
            this.i2cConfig = new I2CDevice.Configuration(this.Address, clockRateKhz);

            if (i2cDevice == null) i2cDevice = new I2CDevice(this.i2cConfig);
        }

        public NetduinoI2CDevice(byte address)
            : this(address, DefaultClockRate)
        {
        }

        private void Write(byte[] writeBuffer)
        {
            i2cDevice.Config = this.i2cConfig;

            // create a write transaction containing the bytes to be written to the device
            writeTransaction[0] = I2CDevice.CreateWriteTransaction(writeBuffer);

            // write the data to the device
            int written = i2cDevice.Execute(writeTransaction, TransactionTimeout);

            // make sure the data was sent
            if (written != writeBuffer.Length)
            {
                throw new Exception("Could not write to device.");
            }
        }

        private void Read(byte[] readBuffer)
        {
            i2cDevice.Config = this.i2cConfig;

            // create a read transaction
            readTransaction[0] = I2CDevice.CreateReadTransaction(readBuffer);

            // read data from the device
            int read = i2cDevice.Execute(readTransaction, TransactionTimeout);

            // make sure the data was read
            if (read != readBuffer.Length)
            {
                throw new Exception("Could not read from device.");
            }
        }

        public void WriteRegister(byte register, byte value)
        {
            size2[0] = register;
            size2[1] = value;
            this.Write(size2);
        }

        public void WriteRegister(byte register, byte[] values)
        {
            // create a single buffer, so register and values can be send in a single transaction
            byte[] writeBuffer = new byte[values.Length + 1];
            writeBuffer[0] = register;
            Array.Copy(values, 0, writeBuffer, 1, values.Length);

            this.Write(writeBuffer);
        }

        public void ReadRegister(byte register, byte[] readBuffer)
        {
            size1[0] = register;
            this.Write(size1);
            this.Read(readBuffer);
        }

        public byte ReadRegister(byte register)
        {
            ReadRegister(register, readBuffer);
            return readBuffer[0];
        }
    }
}
