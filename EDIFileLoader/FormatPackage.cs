
using System;

namespace EDIFileLoader
{
    public class FormatPackage
    {
        public FormatPackage(string format, string version)
        {

        }

        public FormatPackage() { }

        public string formatPackage { get; set; }

        public DateTime validFrom { get; set; }
        public DateTime validTo { get; set; }
    }
}
