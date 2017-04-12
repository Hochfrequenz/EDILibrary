using System;
using System.Collections.Generic;
using System.Text;

namespace EDILibrary.Exceptions
{
    public class BadPIDException : Exception
    {
        string _pid = null;
        public override string Message {get{ return "Pid " + _pid + " is unknown."; } }
        public BadPIDException(string pid)
        {
            _pid = pid;
        }
    }
}
