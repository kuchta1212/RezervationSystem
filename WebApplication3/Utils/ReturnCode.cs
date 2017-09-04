using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationSystem.Utils
{
    public class ReturnCode
    {
        public ReturnCodeLevel ReturnLevel { get; private set; }

        public string Message { get; private set; }

        public string Reason { get; private set; }

        public ReturnCode(ReturnCodeLevel level, string message, string reason)
        {
            this.ReturnLevel = level;
            this.Message = message;
            this.Reason = reason;
        }

        public ReturnCode()
        {
            this.ReturnLevel = ReturnCodeLevel.RELOAD;
            this.Message = Resource.ReloadOK;
            this.Reason = string.Empty;
        }

        public void Error(string message)
        {
            this.ReturnLevel = ReturnCodeLevel.ERROR;
            this.Message = message;
        }
    }
}