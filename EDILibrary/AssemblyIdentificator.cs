using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EDILibrary
{
    public class AssemblyIdentificator
    {
        public Assembly GetAssembly()
        {
            return Assembly.GetEntryAssembly();
        }
    }
}
