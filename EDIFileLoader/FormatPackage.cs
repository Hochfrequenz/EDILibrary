using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace EDIFileLoader
{
    public class FormatPackage : TableEntity
    {
        public FormatPackage(string format, string version)
        {
            this.PartitionKey = format;
            this.RowKey = version;
        }

        public FormatPackage() { }

        public string formatPackage { get; set; }

        public DateTime validFrom { get; set; }
        public DateTime validTo { get; set; }
    }
}
