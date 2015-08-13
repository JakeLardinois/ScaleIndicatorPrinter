using System;
using Microsoft.SPOT;

namespace ScaleIndicatorPrinter.Models
{
    public class MCP23017
    {
        public enum Command : byte
        {
            MCP23017_IODIRA = 0x00,
            MCP23017_IPOLA = 0x02,
            MCP23017_GPINTENA = 0x04,
            MCP23017_DEFVALA = 0x06,
            MCP23017_INTCONA = 0x08,
            MCP23017_IOCONA = 0x0A,
            MCP23017_GPPUA = 0x0C,
            MCP23017_INTFA = 0x0E,
            MCP23017_INTCAPA = 0x10,
            MCP23017_GPIOA = 0x12,
            MCP23017_OLATA = 0x14,


            MCP23017_IODIRB = 0x01,
            MCP23017_IPOLB = 0x03,
            MCP23017_GPINTENB = 0x05,
            MCP23017_DEFVALB = 0x07,
            MCP23017_INTCONB = 0x09,
            MCP23017_IOCONB = 0x0B,
            MCP23017_GPPUB = 0x0D,
            MCP23017_INTFB = 0x0F,
            MCP23017_INTCAPB = 0x11,
            MCP23017_GPIOB = 0x13,
            MCP23017_OLATB = 0x15
        }

        public enum Direction : byte
        {
            Input = 0x01,
            Output = 0x00
        }

        public NetduinoI2CDevice i2cDevice { get; private set; }

        // Statically allocate a byte array to reduce GC pressure
        // We are on embedded after all ;)
        private byte[] size2 = new byte[2];

        public MCP23017(byte address = 0x20)
        {
            i2cDevice = new NetduinoI2CDevice(address);

            // set the defaults
            i2cDevice.WriteRegister((byte)Command.MCP23017_IODIRA, 0xff);
            i2cDevice.WriteRegister((byte)Command.MCP23017_IODIRB, 0xff);

            //i2cDevice configuration register
            // mirror interrupts, disable sequential mode
            i2cDevice.WriteRegister((byte)Command.MCP23017_IOCONA, 0x60); // 0b01100000 (binary) = ox60 (hexadecimal)

            // enable pull-up on switches
            i2cDevice.WriteRegister((byte)Command.MCP23017_GPPUA, 0xff); // pull-up resistor for switch - both ports

            // invert polarity
            i2cDevice.WriteRegister((byte)Command.MCP23017_IPOLA, 0xff); // invert polarity of signal - both ports

            // enable all interrupts
            i2cDevice.WriteRegister((byte) Command.MCP23017_GPINTENA, 0xff);
            //i2cDevice.WriteRegister((byte)0x12, 0xff);
        }

        public void PinMode(byte pin, Direction direction)
        {
            // we only have 16 pins, so return if out of range
            if (pin > 15) return;

            // find out which bank we are modifying, and ensure the pin is mod 8
            byte io_addr = (byte)(pin < 8 ? Command.MCP23017_IODIRA : Command.MCP23017_IODIRB);
            pin = (byte)(pin % 8);

            // read the current io direction
            byte io_dir = i2cDevice.ReadRegister(io_addr);

            // update the pin direction
            if (direction == Direction.Input) io_dir |= (byte)(1 << pin);
            else io_dir &= (byte)(~(1 << pin));

            // write the new io direction
            i2cDevice.WriteRegister(io_addr, io_dir);
        }

        public ushort ReadGpioAB()
        {
            // read GPIOAB by reading two successive i2c bytes
            i2cDevice.ReadRegister((byte)Command.MCP23017_GPIOA, size2);
            return (ushort)(size2[0] | (size2[1] << 8));
        }

        public void WriteGpioAB(ushort data)
        {
            // write GPIOAB by writing two successive i2c bytes
            size2[0] = (byte)(data & 0xff);
            size2[1] = (byte)(data >> 8);
            i2cDevice.WriteRegister((byte)Command.MCP23017_GPIOA, size2);
        }

        public void DigitalWrite(byte pin, byte value)
        {
            // we only have 16 pins, so return if out of range
            if (pin > 15) return;

            // find out which bank we are modifying, and ensure the pin is mod 8
            byte olat_addr = (byte)(pin < 8 ? Command.MCP23017_OLATA : Command.MCP23017_OLATB);
            byte gpio_addr = (byte)(pin < 8 ? Command.MCP23017_GPIOA : Command.MCP23017_GPIOB);
            pin = (byte)(pin % 8);

            // read the current GPIO output latches
            byte gpio = i2cDevice.ReadRegister(olat_addr);

            // update the pin value
            if (value == 0x01) gpio |= (byte)(1 << pin);
            else gpio &= (byte)(~(1 << pin));

            // write the new gpio value
            i2cDevice.WriteRegister(gpio_addr, gpio);
        }

        public void PullUp(byte pin, byte value)
        {
            // we only have 16 pins, so return if out of range
            if (pin > 15) return;

            // find out which bank we are modifying, and ensure the pin is mod 8
            byte gppu_addr = (byte)(pin < 8 ? Command.MCP23017_GPPUA : Command.MCP23017_GPPUB);
            pin = (byte)(pin % 8);

            // read the current GPIO output latches
            byte gppu = i2cDevice.ReadRegister(gppu_addr);

            // update the pin value
            if (value == 0x01) gppu |= (byte)(1 << pin);
            else gppu &= (byte)(~(1 << pin));

            // write the new gpio value
            i2cDevice.WriteRegister(gppu_addr, gppu);
        }

        public byte DigitalRead(byte pin)
        {
            // we only have 16 pins, so return if out of range
            if (pin > 15) return 0;

            // find out which bank we are modifying, and ensure the pin is mod 8
            byte gpio_addr = (byte)(pin < 8 ? Command.MCP23017_GPIOA : Command.MCP23017_GPIOB);
            pin = (byte)(pin % 8);

            return (byte)((i2cDevice.ReadRegister(gpio_addr) >> pin) & 0x01);
        }
    }
}
