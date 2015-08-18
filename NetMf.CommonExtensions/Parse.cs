#region License
// Copyright (c) 2010 Ross McDermott
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
#endregion

using System;
using Microsoft.SPOT;

namespace NetMf.CommonExtensions
{
    /// <summary>
    /// Provides additional parsing operations
    /// </summary>
    public abstract class Parse
    {
        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseInt(string s, out int i)
        {
            i = 0;
            try
            {
                i = int.Parse(s);
                return true;
            }
            catch 
            {
                return false;
            }    
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseShort(string s, out short i)
        {
            i = 0;
            try
            {
                i = short.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseLong(string s, out long i)
        {
            i = 0;
            try
            {
                i = long.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseDouble(string s, out double i)
        {
            i = 0;
            try
            {
                i = double.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="val">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseBool(string s, out bool val)
        {
            val = false;
            try
            {
                if (s == "1" || s.ToUpper() == bool.TrueString.ToUpper())
                {
                    val = true;

                    return true;
                }
                else if (s == "0" || s.ToUpper() == bool.FalseString.ToUpper())
                {
                    val = false;

                    return true;
                }

                return false;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseUInt(string s, out uint i)
        {
            i = 0;
            try
            {
                i = uint.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseUShort(string s, out ushort i)
        {
            i = 0;
            try
            {
                i = ushort.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to parse the provided string value.
        /// </summary>
        /// <param name="s">String value to be parsed</param>
        /// <param name="i">Variable to set successfully parsed value to</param>
        /// <returns>True if parsing was successful</returns>
        public static bool TryParseULong(string s, out ulong i)
        {
            i = 0;
            try
            {
                i = ulong.Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }



    }

}
