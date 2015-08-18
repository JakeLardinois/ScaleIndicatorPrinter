using Microsoft.SPOT;
using System;
using System.Threading;

namespace NetduinoRGBLCDShield
{
    public enum BacklightColor : byte
    {
        Red = 0x1,
        Yellow = 0x3,
        Green = 0x2,
        Teal = 0x6,
        Blue = 0x4,
        Violet = 0x5,
        White = 0x7
    }

    [Flags]
    public enum Button : byte
    {
        Up = 0x08,
        Down = 0x04,
        Left = 0x10,
        Right = 0x02,
        Select = 0x01
    }

    public class RGBLCDShield
    {
        private MCP23017 mcp23017;
        private const byte rsPin = 15;
        private const byte rwPin = 14;
        private const byte enablePin = 13;
        private static readonly byte[] dataPins = new byte[] { 12, 11, 10, 9 };
        private static readonly byte[] buttonPins = new byte[] { 0, 1, 2, 3, 4 };
        private static readonly byte[] rowStart = new byte[] { 0x00, 0x40, 0x14, 0x54 };

        [Flags]
        public enum DisplayFunction : byte
        {
            LCD_4BITMODE = 0x00,
            LCD_8BITMODE = 0x10,
            LCD_1LINE = 0x00,
            LCD_2LINE = 0x08,
            LCD_5x8DOTS = 0x00,
            LCD_5x10DOTS = 0x04
        }

        [Flags]
        public enum DisplayControl : byte
        {
            LCD_DISPLAYON = 0x04,
            LCD_DISPLAYOFF = 0x00,
            LCD_CURSORON = 0x02,
            LCD_CURSOROFF = 0x00,
            LCD_BLINKON = 0x01,
            LCD_BLINKOFF = 0x00
        }

        [Flags]
        public enum DisplayMode : byte
        {
            LCD_ENTRYRIGHT = 0x00,
            LCD_ENTRYLEFT = 0x02,
            LCD_ENTRYSHIFTINCREMENT = 0x01,
            LCD_ENTRYSHIFTDECREMENT = 0x00
        }

        public enum Commands : byte
        {
            LCD_CLEARDISPLAY = 0x01,
            LCD_RETURNHOME = 0x02,
            LCD_ENTRYMODESET = 0x04,
            LCD_DISPLAYCONTROL = 0x08,
            LCD_CURSORSHIFT = 0x10,
            LCD_FUNCTIONSET = 0x20,
            LCD_SETCGRAMADDR = 0x40,
            LCD_SETDDRAMADDR = 0x80
        }

        private DisplayFunction displayFunction;
        private DisplayControl displayControl;
        private DisplayMode displayMode;

        public RGBLCDShield(MCP23017 mcp23017, byte cols = 16, byte rows = 2, byte dotsize = 0)
        {
            this.mcp23017 = mcp23017;
            this.displayFunction = 0x00;

            // initialize the MCP23017 for this board
            mcp23017.PinMode(8, MCP23017.Direction.Output);
            mcp23017.PinMode(6, MCP23017.Direction.Output);
            mcp23017.PinMode(7, MCP23017.Direction.Output);

            SetBacklight(BacklightColor.White);

            mcp23017.PinMode(rsPin, MCP23017.Direction.Output);
            mcp23017.PinMode(rwPin, MCP23017.Direction.Output);
            mcp23017.PinMode(enablePin, MCP23017.Direction.Output);

            for (int i = 0; i < dataPins.Length; i++) mcp23017.PinMode(dataPins[i], MCP23017.Direction.Output);

            for (int i = 0; i < buttonPins.Length; i++)
            {
                mcp23017.PinMode(buttonPins[i], MCP23017.Direction.Input);
                mcp23017.PullUp(buttonPins[i], 0x01);
            }

            Thread.Sleep(50);
            mcp23017.DigitalWrite(rsPin, 0x00);
            mcp23017.DigitalWrite(enablePin, 0x00);
            mcp23017.DigitalWrite(rwPin, 0x00);

            // put the LCD screen into 4 bit mode
            Write4Bits(0x03);
            Thread.Sleep(5);

            Write4Bits(0x03);
            Thread.Sleep(1);

            Write4Bits(0x03);
            Write4Bits(0x02);

            this.displayFunction = DisplayFunction.LCD_2LINE | DisplayFunction.LCD_4BITMODE | DisplayFunction.LCD_5x8DOTS;
            command((byte)((byte)Commands.LCD_FUNCTIONSET | (byte)this.displayFunction));

            this.displayControl = DisplayControl.LCD_DISPLAYON | DisplayControl.LCD_CURSOROFF | DisplayControl.LCD_BLINKOFF;
            Display();

            Clear();

            this.displayMode = DisplayMode.LCD_ENTRYLEFT | DisplayMode.LCD_ENTRYSHIFTDECREMENT;
            command((byte)((byte)Commands.LCD_ENTRYMODESET | (byte)this.displayMode));
        }

        public void Display()
        {
            this.displayControl |= DisplayControl.LCD_DISPLAYON;
            command((byte)((byte)Commands.LCD_DISPLAYCONTROL | (byte)this.displayControl));
        }

        public void NoDisplay()
        {
            this.displayControl &= ~DisplayControl.LCD_DISPLAYON;
            command((byte)((int)Commands.LCD_DISPLAYCONTROL | (int)this.displayControl));
        }

        public void Clear()
        {
            command(Commands.LCD_CLEARDISPLAY);
            Thread.Sleep(2);
        }

        public void Home()
        {
            command(Commands.LCD_RETURNHOME);
            Thread.Sleep(2);
        }

        private void command(byte command)
        {
            SendCommand(command, 0x00);
        }

        private void command(Commands command)
        {
            SendCommand((byte)command, 0x00);
        }

        private void write(byte value)
        {
            SendCommand(value, 0x01);
        }

        public void SetPosition(byte row, byte column)
        {
            byte data = 0x80;
            data += (byte)(rowStart[row] + column);
            SendCommand(data, 0x00);
        }

        public void Write(string data, bool mode = false)
        {
            byte[] buffer = new byte[data.Length];
            for (int i = 0; i < data.Length; i++) buffer[i] = (byte)data[i];
            this.write(buffer, mode);
        }

        private void write(byte[] value, bool mode)
        {
            mcp23017.DigitalWrite(rsPin, (byte)(mode ? 0x00 : 0x01));

            for (int i = 0; i < value.Length; i++)
            {
                Write4Bits(value[i] >> 4);
                Write4Bits(value[i] & 0x0f);
            }
        }

        private void SendCommand(byte value, byte mode)
        {
            mcp23017.DigitalWrite(rsPin, mode);
            mcp23017.DigitalWrite(rwPin, 0x00);

            Write4Bits(value >> 4);
            Write4Bits(value & 0x0f);
        }

        private void Write4Bits(int value)
        {
            ushort gpio = mcp23017.ReadGpioAB();

            for (int i = 0; i < 4; i++)
            {
                gpio &= (ushort)(~(1 << dataPins[i]));
                gpio |= (ushort)(((value >> i) & 0x01) << dataPins[i]);
            }

            mcp23017.WriteGpioAB(gpio);

            gpio |= (ushort)(1 << enablePin);
            mcp23017.WriteGpioAB(gpio);

            gpio &= (ushort)((~(1 << enablePin)) & 0xFFFF); // who promotes to int on bit negation?!?
            mcp23017.WriteGpioAB(gpio);
        }

        public void SetBacklight(BacklightColor color)
        {
            byte status = (byte)color;
            mcp23017.DigitalWrite(8, (byte)(~(status >> 2) & 0x01));
            mcp23017.DigitalWrite(7, (byte)(~(status >> 1) & 0x01));
            mcp23017.DigitalWrite(6, (byte)(~status & 0x01));
        }

        public Button ReadButtons()
        {
            return (Button)(~mcp23017.i2cDevice.ReadRegister((byte)MCP23017.Command.MCP23017_GPIOA) & 0x1f);
        }
    }
}
