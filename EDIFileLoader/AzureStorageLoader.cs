﻿// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using EDILibrary.Interfaces;
using EDILibrary;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EDIFileLoader
{
    public class AzureStorageLoader : EDILibrary.Interfaces.ITemplateLoader
    {
        protected string _accountName;
        protected string _accountKey;
        protected string _containerName;
        protected string _connectionString;

        protected CloudStorageAccount _storageAccount;
        protected CloudBlobClient _blobClient;
        protected CloudTableClient _tableClient;
        protected CloudBlobContainer _container;
        protected CloudTable _formatTable;
        public AzureStorageLoader(string accountName, string accountKey, string containerName,string connectionString)
        {
            _accountKey = accountKey;
            _accountName = accountName;
            _containerName = containerName;
            // Parse the connection string and return a reference to the storage account.
            _storageAccount = CloudStorageAccount.Parse(
                connectionString);

            _blobClient = _storageAccount.CreateCloudBlobClient();

            _tableClient = _storageAccount.CreateCloudTableClient();
            _formatTable =  _tableClient.GetTableReference("formatPackage");
            _container = _blobClient.GetContainerReference(containerName);

        }
        public async Task<string> LoadEDITemplate(EDIFileInfo info, string type)
        {
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(System.IO.Path.Combine("edi", info.Format, info.Format + info.Version + "." + type));
            string text = await blockBlob.DownloadTextAsync();
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (text.StartsWith(_byteOrderMarkUtf8))
            {
                text = text.Remove(0, _byteOrderMarkUtf8.Length);
            }
            return text;
        }
        public async Task<string> LoadJSONTemplate(string fileName)
        {
            throw new NotSupportedException();
        }
        public async Task<string> LoadJSONTemplate(string formatPackage, string fileName)
        {
            if(formatPackage.Contains("|")) //New special case in that a formatPackage is not given directly but a format version string
            {
                //try to read from Azure
                TableResult result = await _formatTable.ExecuteAsync(TableOperation.Retrieve<FormatPackage>(formatPackage.Split('|')[0],formatPackage.Split('|')[1]));
                formatPackage = ((FormatPackage)result.Result).formatPackage;
                
            }
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(System.IO.Path.Combine(formatPackage.Replace("/",""),fileName).Replace("\\", "/"));
            BlobRequestOptions options = new BlobRequestOptions()
            {
                DisableContentMD5Validation = true,
            };
            string text = await blockBlob.DownloadTextAsync(System.Text.Encoding.UTF8, null,options,null);

            byte[] preamble = Encoding.UTF8.GetPreamble();
            string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(preamble);
            if (text.StartsWith(_byteOrderMarkUtf8) && Encoding.UTF8.GetBytes(text)[0]==preamble[0])
            {
                text = text.Remove(0, _byteOrderMarkUtf8.Length);
            }
            return text;

        }
    }
}
