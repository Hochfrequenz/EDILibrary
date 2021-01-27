// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using Azure.Storage.Blobs;

using EDILibrary;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EDIFileLoader
{
    public class AzureStorageLoader : EDILibrary.Interfaces.ITemplateLoader
    {
        protected string _accountName;
        protected string _accountKey;
        protected string _containerName;
        protected string _connectionString;


        protected BlobServiceClient _blobClient;
        protected BlobContainerClient _container;

        public AzureStorageLoader(string accountName, string accountKey, string containerName, string connectionString)
        {
            _accountKey = accountKey;
            _accountName = accountName;
            _containerName = containerName;
            // Parse the connection string and return a reference to the storage account.
            _blobClient = new BlobServiceClient(connectionString);

            _container = _blobClient.GetBlobContainerClient(_containerName);
        }
        public async Task<string> LoadEDITemplate(EDIFileInfo info, string type)
        {
            var blockBlob = _container.GetBlobClient(System.IO.Path.Combine("edi", info.Format, info.Format + info.Version + "." + type));

            string text = await new StreamReader((await blockBlob.DownloadAsync()).Value.Content, Encoding.UTF8).ReadToEndAsync();
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (text.StartsWith(_byteOrderMarkUtf8))
            {
                text = text.Remove(0, _byteOrderMarkUtf8.Length);
            }
            return text;
        }
        public Task<string> LoadJSONTemplate(string fileName)
        {
            throw new NotSupportedException();
        }
        public async Task<string> LoadJSONTemplate(string formatPackage, string fileName)
        {
            var blockBlob = _container.GetBlobClient(System.IO.Path.Combine(formatPackage.Replace("/", ""), fileName).Replace("\\", "/"));

            string text = await new StreamReader((await blockBlob.DownloadAsync()).Value.Content, Encoding.UTF8).ReadToEndAsync();
            byte[] preamble = Encoding.UTF8.GetPreamble();
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(preamble);
            if (text.StartsWith(_byteOrderMarkUtf8) && Encoding.UTF8.GetBytes(text)[0] == preamble[0])
            {
                text = text.Remove(0, _byteOrderMarkUtf8.Length);
            }
            return text;

        }
    }
}
