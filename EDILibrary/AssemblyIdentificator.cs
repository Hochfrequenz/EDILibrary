// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using System.Reflection;

namespace EDILibrary
{
    public class AssemblyIdentificator
    {
        public static Assembly GetAssembly()
        {
            return Assembly.GetEntryAssembly();
        }
    }
}
