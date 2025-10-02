// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using EDILibrary;

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

        protected ConcurrentDictionary<string, Dictionary<string, string>> Cache { get; set; } =
            new ConcurrentDictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// JsonSerializer options
        /// </summary>
        protected JsonSerializerOptions JsonOptions { get; set; }

        public AzureStorageLoader(
            string accountName,
            string accountKey,
            string containerName,
            string connectionString
        )
        {
            _accountKey = accountKey;
            _accountName = accountName;
            _containerName = containerName;
            // Parse the connection string and return a reference to the storage account.
            _blobClient = new BlobServiceClient(connectionString);

            _container = _blobClient.GetBlobContainerClient(_containerName);
            JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public async Task PreloadCache()
        {
            var tasks = new ConcurrentBag<Task>();
            await foreach (
                var prefixPage in _container
                    .GetBlobsByHierarchyAsync(
                        Azure.Storage.Blobs.Models.BlobTraits.Metadata,
                        Azure.Storage.Blobs.Models.BlobStates.None,
                        "/"
                    )
                    .AsPages()
            )
            {
                foreach (var prefix in prefixPage.Values)
                {
                    tasks.Add(
                        Task.Run(async () =>
                        {
                            if (prefix.IsPrefix)
                            {
                                Cache.TryAdd(
                                    prefix.Prefix.TrimEnd('/'),
                                    new Dictionary<string, string>()
                                );
                                await foreach (
                                    var blobPage in _container
                                        .GetBlobsByHierarchyAsync(
                                            Azure.Storage.Blobs.Models.BlobTraits.Metadata,
                                            Azure.Storage.Blobs.Models.BlobStates.None,
                                            null,
                                            prefix.Prefix
                                        )
                                        .AsPages()
                                )
                                {
                                    foreach (var blob in blobPage.Values)
                                    {
                                        if (!blob.IsBlob)
                                        {
                                            continue;
                                        }

                                        var blockBlob = _container.GetBlobClient(blob.Blob.Name);
                                        // if this stream reader fails, because the blockBlob comes without a ByteOrderMark, then use new UTF8Encoding(false) as encoding
                                        string text = await new StreamReader(
                                            (await blockBlob.DownloadAsync()).Value.Content,
                                            Encoding.UTF8
                                        ).ReadToEndAsync();
                                        text = EDIHelper.RemoveByteOrderMark(text);
                                        Cache[prefix.Prefix.TrimEnd('/')]
                                            .TryAdd(blob.Blob.Name, text);
                                    }
                                }
                            }
                        })
                    );
                }
            }
            await Task.WhenAll(tasks);
        }

        public async Task<string> LoadEDITemplate(EDIFileInfo info, string type)
        {
            if (Cache != null)
            {
                try
                {
                    return Cache["edi"][
                        Path.Combine(
                                "edi",
                                info.Format.ToString(),
                                info.Format.ToString() + info.Version + "." + type
                            )
                            .Replace("\\", "/")
                    ];
                }
                catch (KeyNotFoundException) { }
            }
            var blockBlob = _container.GetBlobClient(
                Path.Combine(
                    "edi",
                    info.Format.ToString(),
                    info.Format.ToString() + info.Version + "." + type
                )
            );
            string text = await new StreamReader(
                (await blockBlob.DownloadAsync()).Value.Content,
                Encoding.UTF8
            ).ReadToEndAsync();
            text = EDIHelper.RemoveByteOrderMark(text);
            return text;
        }

        public Task<string> LoadJSONTemplate(string fileName)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Use strongly typed version instead.")]
        public async Task<string> LoadJSONTemplate(string formatPackage, string fileName)
        {
            var format = Enum.Parse<EdifactFormat>(formatPackage.Split("|").First());
            string version = formatPackage.Split("|").Last();
            return await LoadJSONTemplate(format, version, fileName);
        }

        public async Task<string> LoadJSONTemplate(
            EdifactFormat? format,
            string version,
            string fileName
        )
        {
            if (Cache != null)
            {
                try
                {
                    return Cache["edi"][
                        Path.Combine(version.Replace("/", ""), fileName.Replace("\\", "/"))
                    ];
                }
                catch (KeyNotFoundException) { }
            }
            var blockBlob = _container.GetBlobClient(
                Path.Combine(version.Replace("/", ""), fileName).Replace("\\", "/")
            );

            string text = await new StreamReader(
                (await blockBlob.DownloadAsync()).Value.Content,
                Encoding.UTF8
            ).ReadToEndAsync();
            text = EDIHelper.RemoveByteOrderMark(text);
            return text;
        }

        /// <summary>
        /// <see cref="EDILibrary.Interfaces.TemplateLoader.LoadMausTemplate"/>
        /// </summary>
        public async Task<EDILibrary.MAUS.Anwendungshandbuch> LoadMausTemplate(
            EdifactFormat? format,
            EdifactFormatVersion version,
            string pid
        )
        {
            if (Cache != null)
            {
                try
                {
                    return JsonSerializer.Deserialize<EDILibrary.MAUS.Anwendungshandbuch>(
                        Cache["maus"][
                            Path.Combine(version.ToString(), format.ToString(), pid + "_maus.json")
                        ],
                        JsonOptions
                    );
                }
                catch (KeyNotFoundException) { }
            }
            var blockBlob = _container.GetBlobClient(
                Path.Combine("maus", version.ToString(), format.ToString(), pid + "_maus.json")
            );

            string text = await new StreamReader(
                (await blockBlob.DownloadAsync()).Value.Content,
                Encoding.UTF8
            ).ReadToEndAsync();
            text = EDIHelper.RemoveByteOrderMark(text);
            return JsonSerializer.Deserialize<EDILibrary.MAUS.Anwendungshandbuch>(
                text,
                JsonOptions
            );
        }
    }
}
