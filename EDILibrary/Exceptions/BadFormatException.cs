// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Text;

namespace EDILibrary.Exceptions
{
    public class BadFormatException : Exception
    {
        string _format = null;
        string _version = null;
        public override string Message {get{ return String.Format("Format {0} in version {1} could not be found",_format,_version); } }
        public BadFormatException(string format, string version)
        {
            _format = format;
            _version = version;
        }
    }
}
