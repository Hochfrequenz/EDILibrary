﻿// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;

namespace EDILibrary.Exceptions
{
    public class BadPIDException : Exception
    {
        string _pid;
        public override string Message {get{ return "Pid " + _pid + " is unknown."; } }
        public BadPIDException(string pid)
        {
            _pid = pid;
        }
    }
}
