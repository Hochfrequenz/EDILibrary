// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;

namespace EDILibrary.Exceptions
{
    public class BadFormatException : Exception
    {
        string _format;
        string _version;
        public override string Message { get { return string.Format("Format {0} in version {1} could not be found", _format, _version); } }
        public BadFormatException(string format, string version)
        {
            _format = format;
            _version = version;
        }
    }
}
