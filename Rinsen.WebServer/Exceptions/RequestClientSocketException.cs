using System;
using Microsoft.SPOT;

namespace Rinsen.WebServer.Exceptions
{
    public class RequestSocketException : Exception
    {
        public RequestSocketException(string message)
            : base (message)
        { }
    }
}
