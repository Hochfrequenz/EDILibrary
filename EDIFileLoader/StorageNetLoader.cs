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

using EDILibrary;

using Microsoft.Extensions.Logging;
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
        /// <summary>
        /// JsonSerializer options
        /// </summary>
        protected JsonSerializerOptions JsonOptions { get; set; }
        public StorageNetLoader(ILogger<StorageNetLoader> logger, Storage.Net.Blobs.IBlobStorage storage, string root = "/")
        {
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Logger = logger;
            Root = root;
            JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
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
            await using var s = new MemoryStream();
            Logger.LogDebug($"Reading path {path}");
            await using Stream ss = await Storage.OpenReadAsync(path);
            Logger.LogDebug("Copying to stream");
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
                catch (KeyNotFoundException)
                {
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
                    {
                        ediCache = new Dictionary<string, string>();
                    }

                    ediCache[Path.Combine("edi", info.Format.ToString(), info.Format.ToString() + info.Version + "." + type).Replace("\\", "/")] = text;
                }
                return text;
            }
            catch (Exception exc)
            {
                Logger.LogDebug(exc, $"Could not load edi template from storage: {exc.Message}");
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
        /// <returns>In case of any error (pokemon catcher!) returns an empty string</returns>
        public async Task<string> LoadJSONTemplate(EdifactFormat? format, string version, string fileName)
        {
            if (Cache != null && Cache.Any())
            {
                try
                {
                    return Cache[version.Replace("/", "")][fileName.Replace("\\", "/")];
                }
                catch (KeyNotFoundException)
                {
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
                    {
                        ediCache = new Dictionary<string, string>();
                    }

                    ediCache[fileName.Replace("\\", "/")] = text;
                }
                return text;
            }
            catch (Exception exc)
            {
                Logger.LogWarning(exc, $"Could not load edi template from storage: {exc.Message}");
                // why should we raise a meaningful error message when we can just return an empty string and fail somewhere else instead of the place where the error occured?
                return "";
            }
        }
        /// <summary>
        /// <see cref="EDILibrary.Interfaces.TemplateLoader.LoadMausTemplate"/>
        /// </summary>
        public async Task<EDILibrary.MAUS.Anwendungshandbuch> LoadMausTemplate(EdifactFormat? format, EdifactFormatVersion version, string pid, bool useOlderVersionsAsFallback)
        {
            if (Cache != null && Cache.Any())
            {
                var mausPath = Path.Combine(version.ToString(), format.ToString(), pid + "_maus.json");
                try
                {
                    return JsonSerializer.Deserialize<EDILibrary.MAUS.Anwendungshandbuch>(Cache["maus"][mausPath], JsonOptions);
                }
                catch (KeyNotFoundException)
                {
                    Logger.LogDebug("The maus {MausPath} was not found in the cache", mausPath);
                }
                catch (JsonException jsonException)// why shouldn't it be valid json?
                {
                    Logger.LogWarning("Invalid Json found: {JsonExceptionMessage}", jsonException.Message);
                }
            }
            try
            {
                var mausPath = Path.Combine(Root != "/" ? Root : "", "maus", version.ToString(), format.ToString(), pid + "_maus.json");
                string fileContent = string.Empty;
                try
                {
                     fileContent = await GetUTF8TextFromPath(mausPath);
                }
                // sorry for the broad except / pokemon catchers.
                // I just don't know what kind of exception GetUTF8TextFromPath raises in case the path does not exist.
                catch(Exception fnfe) when (useOlderVersionsAsFallback && version!=EdifactFormatVersion.FV1710)
                {
                    var olderVersions = Enum.GetValues(typeof(EdifactFormatVersion))
                        .Cast<EdifactFormatVersion>().Where(fv => fv < version)
                        .OrderByDescending(fv=>fv);
                    foreach (var olderVersion in olderVersions)
                    {
                        var fallbackPath = mausPath.Replace(version.ToString(), olderVersion.ToString());
                        Logger.LogDebug("Trying to load fallback maus from {FallbackPath}", fallbackPath);
                        try
                        {
                            fileContent = await GetUTF8TextFromPath(fallbackPath);
                            Logger.LogWarning("Using fallback maus from {FallbackPath} (instead of requested {MausPath})", fallbackPath, mausPath);
                        }
                        catch (Exception) when (olderVersion == EdifactFormatVersion.FV1710)
                        {
                            throw fnfe; // no fallback found
                        }
                    }
                }

                fileContent = EDIHelper.RemoveByteOrderMark(fileContent);
                if (Cache != null)
                {
                    if (!Cache.ContainsKey("maus"))
                    {
                        Cache["maus"] = new Dictionary<string, string>();
                    }
                    var ediCache = Cache["maus"];
                    if (ediCache == null)
                    {
                        ediCache = new Dictionary<string, string>();
                    }

                    ediCache[Path.Combine(version.ToString(), format.ToString(), pid + "_maus.json")] = fileContent;
                }
                return JsonSerializer.Deserialize<EDILibrary.MAUS.Anwendungshandbuch>(fileContent, JsonOptions);
            }
            catch (Exception exc)
            {
                Logger.LogDebug(exc, $"Could not load maus template from storage: {exc.Message}");
                return null;
            }
        }
    }
}
