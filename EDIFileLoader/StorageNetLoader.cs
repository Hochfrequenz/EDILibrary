// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using EDILibrary;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDIFileLoader
{
    public class StorageNetLoader : EDILibrary.Interfaces.ITemplateLoader
    {
        /// <summary>
        /// IBlobStorage used to access the underlying blob architecture (azure blob, S3, file etc.)
        /// </summary>
        protected Storage.Net.Blobs.IBlobStorage Storage { get; set; }

        /// <summary>
        /// Root path (e.g. container in azure blob)
        /// </summary>
        protected string Root { get; set; }

        /// <summary>
        /// Logger
        /// </summary>
        protected ILogger Logger { get; set; }
        /// <summary>
        /// Cache dictionary to not download every file again
        /// </summary>
        protected ConcurrentDictionary<string, Dictionary<string, string>> Cache { get; set; } = new ConcurrentDictionary<string, Dictionary<string, string>>();
        public StorageNetLoader(ILogger<StorageNetLoader> logger, Storage.Net.Blobs.IBlobStorage storage, string root = "/")
        {
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Logger = logger;
            Root = root;
        }
        /// <summary>
        /// Loads all templates from the root of the blob in a cache (parallelizes per folder)
        /// </summary>
        /// <returns></returns>
        public async Task PreloadCache()
        {
            var tasks = new ConcurrentBag<Task>();
            // go through all folders
            foreach (var folder in await Storage.ListAsync(new Storage.Net.Blobs.ListOptions() { FolderPath = Root }))
            {

                tasks.Add(Task.Run(async () =>
                 {
                     if (folder.IsFolder)
                     {
                         Cache.TryAdd(folder.Name.TrimEnd('/'), new Dictionary<string, string>());
                         foreach (var blob in await Storage.ListAsync(new Storage.Net.Blobs.ListOptions() { FolderPath = folder.FolderPath }))
                         {
                             if (blob.IsFile)
                             {
                                 var text = await GetUTF8TextFromPath(blob.FullPath);
                                 Cache[folder.Name.TrimEnd('/')].TryAdd(blob.Name, text);
                             }

                         }
                     }
                 }));

            }
            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Helper method to download a UTF-8 string from the blob storage
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected async Task<string> GetUTF8TextFromPath(string path)
        {
            using var s = new MemoryStream();
            Logger.LogDebug($"Reading path {path}");
            using Stream ss = await Storage.OpenReadAsync(path);
            Logger.LogDebug($"Copying to stream");
            await ss.CopyToAsync(s);

            return Encoding.UTF8.GetString(s.ToArray());
        }
        /// <summary>
        /// Load an edifact template specified by file info and type
        /// </summary>
        /// <param name="info"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<string> LoadEDITemplate(EDIFileInfo info, string type)
        {
            if (Cache != null && Cache.Any())
            {
                try
                {
                    return Cache["edi"][Path.Combine("edi", info.Format.ToString(), info.Format.ToString() + info.Version + "." + type).Replace("\\", "/")];
                }
                catch (Exception)
                {
                    // todo: no pokemon-catcher
                }
            }
            try
            {
                var text = await GetUTF8TextFromPath(Path.Combine(Root != "/" ? Root : "", "edi", info.Format.ToString(), info.Format.ToString() + info.Version + "." + type).Replace("\\", "/"));
                text = EDIHelper.RemoveByteOrderMark(text);
                if (Cache != null)
                {
                    if (!Cache.ContainsKey("edi"))
                    {
                        Cache["edi"] = new Dictionary<string, string>();
                    }
                    var ediCache = Cache["edi"];
                    if (ediCache == null)
                        ediCache = new Dictionary<string, string>();
                    ediCache[Path.Combine("edi", info.Format.ToString(), info.Format.ToString() + info.Version + "." + type).Replace("\\", "/")] = text;
                }
                return text;
            }
            catch (Exception exc)
            {
                Logger.LogDebug($"Could not load edi template from storage: {exc}");
                return "";
            }
        }
        [Obsolete("Use strongly typed version instead.", true)]
        public Task<string> LoadJSONTemplate(string fileName)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Use strongly typed version instead.", true)]
        public async Task<string> LoadJSONTemplate(string formatPackage, string fileName)
        {
            var format = Enum.Parse<EdifactFormat>(formatPackage.Split("|").First());
            var version = formatPackage.Split("|").Last();
            return await LoadJSONTemplate(format, version, fileName);
        }
        /// <summary>
        /// Load a template or create.template for a specifig format and version
        /// </summary>
        /// <param name="format"></param>
        /// <param name="version"></param>
        /// <param name="fileName">template/create.template</param>
        /// <returns></returns>
        public async Task<string> LoadJSONTemplate(EdifactFormat? format, string version, string fileName)
        {
            if (Cache != null && Cache.Any())
            {
                try
                {
                    return Cache[version.Replace("/", "")][fileName.Replace("\\", "/")];
                }
                catch (Exception)
                {
                    // todo: no pokemon-catcher
                }
            }
            try
            {
                var text = await GetUTF8TextFromPath(Path.Combine(Root != "/" ? Root : "", version.Replace("/", ""), fileName).Replace("\\", "/"));
                text = EDIHelper.RemoveByteOrderMark(text);
                if (Cache != null)
                {
                    if (!Cache.ContainsKey(version.Replace("/", "")))
                    {
                        Cache[version.Replace("/", "")] = new Dictionary<string, string>();
                    }
                    var ediCache = Cache[version.Replace("/", "")];
                    if (ediCache == null)
                        ediCache = new Dictionary<string, string>();
                    ediCache[fileName.Replace("\\", "/")] = text;
                }
                return text;
            }
            catch (Exception exc)
            {
                Logger.LogDebug($"Could not load edi template from storage: {exc}");
                return "";
            }
        }
    }
}
