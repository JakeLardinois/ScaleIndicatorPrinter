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
    /// Construct a larger string by appending strings together.
    /// </summary>
    public class StringBuilder
    {
        private const int INITIAL_SIZE = 16;
        private const int MIN_GROWTH_SIZE = 64;

        private char[] _content = null;
        private int _currentLength = 0;

        /// <summary>
        /// Public constructor
        /// </summary>
        public StringBuilder() : this(INITIAL_SIZE)
        {
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="capacity">Set initial builder capacity</param>
        public StringBuilder(int capacity)
        {
            this._content = new char[capacity];
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="initital">The initial content of the string builder</param>
        public StringBuilder(string initital)
        {
            this._content = initital.ToCharArray();
            this._currentLength = _content.Length;
        }

        /// <summary>
        /// Append a character to the current string builder
        /// </summary>
        /// <param name="c"></param>
        public void Append(char c)
        {
            this.Append(new string(new char[] { c }));
        }

        /// <summary>
        /// Append a string to the current string builder
        /// </summary>
        /// <param name="toAppend">String to be appended.</param>
        public void Append(string toAppend)
        {
            int additionalSpaceRequired = (toAppend.Length + _currentLength) - _content.Length;
            
            if (additionalSpaceRequired > 0)
            {
                // ensure at least minimum growth size is done to minimise future copying / manipulation
                if (additionalSpaceRequired < MIN_GROWTH_SIZE)
                {
                    additionalSpaceRequired = MIN_GROWTH_SIZE;
                }

                char[] tmp = new char[_content.Length + additionalSpaceRequired];

                // copy content to new array
                Array.Copy(_content, tmp, _currentLength);

                // replace the content array.
                _content = tmp;
            }

            // copy the new content to the holding array
            Array.Copy(toAppend.ToCharArray(), 0, _content, _currentLength, toAppend.Length);
            _currentLength += toAppend.Length;
        }


        /// <summary>
        /// Append the provided line along with a new line.
        /// </summary>
        /// <param name="str"></param>
        public void AppendLine(string str)
        {
            this.Append(str);
            this.Append("\r\n");
        }

        /// <summary>
        /// Append to the string builder using format string and placeholder arguments
        /// </summary>
        /// <param name="format">String to be formatted</param>
        /// <param name="args">Arguments to be placed into the formatted string</param>
        public void AppendFormat(string format, params object[] args)
        {
            this.Append(StringUtility.Format(format, args));
        }

        /// <summary>
        /// Clear the current string builder back to an empty string.
        /// </summary>
        public void Clear()
        {
            _currentLength = 0;
        }

        /// <summary>
        /// Get the final built string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new string(_content, 0, _currentLength);
        }

      

    }
}
