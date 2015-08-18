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
using System.Collections;

namespace NetMf.CommonExtensions
{
    /// <summary>
    /// Provides additional standard string operations
    /// </summary>
    public abstract class StringUtility
    {
        /// <summary>
        /// Check if the provided string is either null or empty
        /// </summary>
        /// <param name="str">String to validate</param>
        /// <returns>True if the string is null or empty</returns>
        public static bool IsNullOrEmpty(string str)
        {
            if (str == null || str == string.Empty)
                return true;

            return false;
        }


        /// <summary>
        /// Replaces one or more format items in a specified string with the string representation of a specified object.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg">The object to format.</param>
        /// <returns>A copy of format in which any format items are replaced by the string representation of arg0.</returns>
        /// <exception cref="NetMf.CommonExtensions.FormatException">format is invalid, or the index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
        /// <exception cref="System.ArgumentNullException">format or args is null</exception>
        public static string Format(string format, object arg)
        {
            return Format(format, new object[] { arg });
        }

        /// <summary>
        /// Format the given string using the provided collection of objects.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        /// <exception cref="NetMf.CommonExtensions.FormatException">format is invalid, or the index of a format item is less than zero, or greater than or equal to the length of the args array.</exception>
        /// <exception cref="System.ArgumentNullException">format or args is null</exception>
        /// <example>
        /// x = StringUtility.Format("Quick brown {0}","fox");
        /// </example>
        public static string Format(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            if (args == null)
                throw new ArgumentNullException("args");

            // Validate the structure of the format string.
            ValidateFormatString(format);

            StringBuilder bld = new StringBuilder();

            int endOfLastMatch = 0;
            int starting = 0;

            while (starting >= 0)
            {
                starting = format.IndexOf('{', starting);

                if (starting >= 0)
                {
                    if (starting != format.Length - 1)
                    {
                        if (format[starting + 1] == '{')
                        {
                            // escaped starting bracket.
                            starting = starting + 2;
                            continue;
                        }
                        else
                        {
                            bool found = false;
                            int endsearch = format.IndexOf('}', starting);

                            while(endsearch > starting)
                            {
                                if (endsearch != (format.Length - 1) && format[endsearch + 1] == '}')
                                {
                                    // escaped ending bracket
                                    endsearch = endsearch + 2;
                                }
                                else
                                {
                                    if(starting != endOfLastMatch)
                                    {
                                        string t = format.Substring(endOfLastMatch, starting - endOfLastMatch);
                                        t = t.Replace("{{", "{"); // get rid of the escaped brace
                                        t = t.Replace("}}", "}"); // get rid of the escaped brace
                                        bld.Append(t);
                                    }

                                    // we have a winner
                                    string fmt = format.Substring(starting, endsearch-starting + 1);

                                    if (fmt.Length >= 3)
                                    {
                                        fmt = fmt.Substring(1, fmt.Length - 2);

                                        string[] indexFormat = fmt.Split(new char[] { ':' });

                                        string formatString = string.Empty;

                                        if (indexFormat.Length == 2)
                                        {
                                            formatString = indexFormat[1];
                                        }

                                        int index = 0;

                                        // no format, just number
                                        if (Parse.TryParseInt(indexFormat[0], out index))
                                        {
                                            bld.Append(FormatParameter(args[index], formatString));
                                        }
                                        else
                                        {
                                            throw new FormatException(FormatException.ERROR_MESSAGE);
                                        }
                                    }

                                    endOfLastMatch = endsearch + 1;

                                    found = true;
                                    starting = endsearch + 1;
                                    break;
                                }


                                endsearch = format.IndexOf('}', endsearch);
                            }
                            // need to find the ending point

                            if(!found)
                            {
                                throw new FormatException(FormatException.ERROR_MESSAGE);
                            }
                        }
                    }
                    else
                    {
                        // invalid
                        throw new FormatException(FormatException.ERROR_MESSAGE);
                    }

                }

            }

            // copy any additional remaining part of the format string.
            if (endOfLastMatch != format.Length)
            {
                bld.Append(format.Substring(endOfLastMatch, format.Length - endOfLastMatch));
            }

            return bld.ToString();
        }

        private static void ValidateFormatString(string format)
        {
            char expected = '{';

            int i = 0;

            while ((i = format.IndexOfAny(new char[] { '{', '}' }, i)) >= 0)
            {
                if (i < (format.Length - 1) && format[i] == format[i + 1])
                {
                    // escaped brace. continue looking.
                    i = i + 2;
                    continue;   
                } 
                else if (format[i] != expected)
                {
                    // badly formed string.
                    throw new FormatException(FormatException.ERROR_MESSAGE);
                }
                else
                {
                    // move it along.
                    i++;

                    // expected it.
                    if (expected == '{')
                        expected = '}';
                    else
                        expected = '{';
                }
            }

            if (expected == '}')
            {
                // orpaned opening brace. Bad format.
                throw new FormatException(FormatException.ERROR_MESSAGE);
            }

        }


        /// <summary>
        /// Format the provided object using the provided format string.
        /// </summary>
        /// <param name="p">Object to be formatted</param>
        /// <param name="formatString">Format string to be applied to the object</param>
        /// <returns>Formatted string for the object</returns>
        private static string FormatParameter(object p, string formatString)
        {
            if (formatString == string.Empty)
                return p.ToString();

            if (p as IFormattable != null)
            {
                return ((IFormattable)p).ToString(formatString,null);
            }
            else if (p is DateTime)
            {
                return ((DateTime)p).ToString(formatString);
            }
            else if (p is Double)
            {
                return ((Double)p).ToString(formatString);
            }
            else if (p is Int16)
            {
                return ((Int16)p).ToString(formatString);
            }
            else if (p is Int32)
            {
                return ((Int32)p).ToString(formatString);
            }
            else if (p is Int64)
            {
                return ((Int64)p).ToString(formatString);
            }
            else if (p is SByte)
            {
                return ((SByte)p).ToString(formatString);
            }
            else if (p is Single)
            {
                return ((Single)p).ToString(formatString);
            }
            else if (p is UInt16)
            {
                return ((UInt16)p).ToString(formatString);
            }
            else if (p is UInt32)
            {
                return ((UInt32)p).ToString(formatString);
            }
            else if (p is UInt64)
            {
                return ((UInt64)p).ToString(formatString);
            }
            else
            {
                return p.ToString();
            }
            
        }


        
    }
}
