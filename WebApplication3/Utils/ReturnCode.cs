using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

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
            this.Reason = message;
            this.Message = Resource.WriteAnAdministrator;
        }

        public override string ToString()
        {
            return (int)this.ReturnLevel + ";" + this.Message + ";" + this.Reason;
        }

        public static ReturnCode FromString(string data)
        {
            if (data == null || data.IsEmpty())
                return null;

            var dataToArray = data.Split(';');

            if (dataToArray.Length != 3)
                return null;
            return new ReturnCode()
            {
                ReturnLevel = (ReturnCodeLevel)(Int32.Parse(dataToArray[0])),
                Message = dataToArray[1],
                Reason = dataToArray[2]
            };

        }
    }
}