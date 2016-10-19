using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCS.Services.IPeople
{
    public class RetryByTimeTimeoutException : Exception
    {
        public RetryByTimeTimeoutException(string message) : base(message) { }
    }
}