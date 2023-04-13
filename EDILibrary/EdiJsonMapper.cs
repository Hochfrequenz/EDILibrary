// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using EDILibrary.Exceptions;
using EDILibrary.Helper;

using Humanizer;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDILibrary
{
    public class EdiJsonMapper
    {
        protected Interfaces.ITemplateLoader _loader;

        public EdiJsonMapper(Interfaces.ITemplateLoader loader)
        {
            _loader = loader;
        }
        public struct JsonResult
        {
            public string EDI;
            public EdifactFormat? Format;
            /// <summary>
            /// e.g. 5.2h
            /// </summary>
            public string Version;
            public string Sender;
            public string Receiver;
        }

        [Obsolete("Use strongly typed version instead")]
        public async Task<string> ParseToJson(string edi, string packageVersion, string includeEmptyValues = null)
        {
            return (await ParseToJsonWithVersion(edi, packageVersion.ToEdifactFormatVersion(), includeEmptyValues)).EDI;
        }
        public async Task<JsonResult> ParseToJsonWithTemplates(string edi, EdifactFormatVersion? packageVersion, string ediTemplate, string ediTreeTemplate, string ediJsonTemplate, string includeEmptyValues = null)
        {
            var ediInfo = EDIHelper.GetEdiFileInfo(edi.Substring(0, Math.Min(1000, edi.Length)));
            var ediString = EDIHelper.NormalizeEDIHeader(edi);
            var templateString = ediTemplate;
            var loader = new GenericEDILoader();
            var template = loader.LoadTemplate(templateString);
            var tree = loader.LoadTree(ediTreeTemplate);
            var ediTree = loader.LoadEDI(ediString, tree);
            var treeHelper = new TreeHelper();
            treeHelper.RefreshDirtyFlags(tree);
            var fileObject = loader.LoadTemplateWithLoadedTree(template, ediTree);
            var jsonResult = JsonConvert.DeserializeObject<JObject>(fileObject.SerializeToJSON());
            Tuple<EdifactFormat?, string> package;
            if (packageVersion.HasValue)
            {
                package = new Tuple<EdifactFormat?, string>(null, packageVersion.Value.ToLegacyVersionString());
            }
            else
            {
                //The template loader can try to read the package from the format and version (and the right table)
                package = new Tuple<EdifactFormat?, string>(ediInfo.Format, ediInfo.Version);
            }
            //mapping laden
            JArray mappings;
            try
            {
                mappings = JsonConvert.DeserializeObject<JArray>(ediJsonTemplate);
            }
            catch (Exception e)
            {
                throw new BadFormatException(ediInfo.Format, ediInfo.Version, e);
            }
            dynamic resultObject = new ExpandoObject();

            // var resultDict = resultObject as IDictionary<string, object>;
            ParseObject(jsonResult, resultObject as IDictionary<string, object>, mappings, includeEmptyValues != null);
            var result = new JsonResult
            {
                EDI = JsonConvert.SerializeObject(resultObject),
                Format = ediInfo.Format,
                Version = ediInfo.Version,
                Sender = ediInfo.Sender.ID,
                Receiver = ediInfo.Empfänger.ID
            };
            return result;
        }
        public async Task<JsonResult> ParseToJsonWithVersion(string edi, EdifactFormatVersion? packageVersion, string includeEmptyValues = null)
        {
            var ediInfo = EDIHelper.GetEdiFileInfo(edi.Substring(0, Math.Min(1000, edi.Length)));
            var treeStringTask = _loader.LoadEDITemplate(ediInfo, "tree");
            var templateStringTask = _loader.LoadEDITemplate(ediInfo, "template");
            await Task.WhenAll(new List<Task> { treeStringTask, templateStringTask });
            var treeString = treeStringTask.Result;
            if (string.IsNullOrWhiteSpace(treeString))
            {
                // something is seriously wrong, don't expect things to work below this line if the treeString is empty
                // just proceed and let things crash later. what should go wrong
            }
            var templateString = templateStringTask.Result;
            Tuple<EdifactFormat?, string> package;
            if (packageVersion.HasValue)
            {
                package = new Tuple<EdifactFormat?, string>(null, packageVersion.Value.ToLegacyVersionString());
            }
            else
            {
                //The template loader can try to read the package from the format and version (and the right table)
                package = new Tuple<EdifactFormat?, string>(ediInfo.Format, ediInfo.Version);
            }
            //mapping laden
            string mappingJson = null;
            try
            {
                mappingJson = await _loader.LoadJSONTemplate(package.Item1, package.Item2, ediInfo.Format + ".json");
            }
            catch (Exception e)
            {
                throw new BadFormatException(ediInfo.Format, ediInfo.Version, e);
            }
            return await ParseToJsonWithTemplates(edi, packageVersion, templateString, treeString, mappingJson, includeEmptyValues);
        }

        [Obsolete("Use strongly typed overload instead.")]
        public async Task<string> CreateFromJson(string jsonInput, string pid, string formatPackage, TimeZoneInfo localTime, MAUS.Anwendungshandbuch? ahb, bool convertFromUTC = false)
        {
            return await CreateFromJson(jsonInput, pid, formatPackage.ToEdifactFormatVersion(), localTime, ahb, convertFromUTC);
        }
        public string CreateFromJsonWithTemplates(string jsonInput, string pid, EdifactFormatVersion formatPackage, string ediJsonTemplate, string createTemplate, TimeZoneInfo localTime, MAUS.Anwendungshandbuch? ahb, bool convertFromUTC = false)
        {
            var format = EdifactFormatHelper.FromPruefidentifikator(pid);

            var mappingsBody = ediJsonTemplate;

            var inputJson = JsonConvert.DeserializeObject<JObject>(jsonInput);
            var mappings = JsonConvert.DeserializeObject<JArray>(mappingsBody);
            var version = mappings[0]["_meta"]["version"].Value<string>();
            var outputJson = CreateMsgJSON(inputJson, mappings, null, ahb?.Lines?.FirstOrDefault(), null, false, new Stack<string>(), out var subParent, convertFromUTC);
            EdiObject result = EdiObject.CreateFromJSON(JsonConvert.SerializeObject(outputJson));
            //apply scripts

            return MappingHelper.ExecuteMappings(result, new EDIFileInfo
            {
                Format = format,
                Version = version,
            }, new List<string>(), createTemplate, localTime, convertFromUTC);
        }
        public async Task<string> CreateFromJson(string jsonInput, string pid, EdifactFormatVersion formatPackage, TimeZoneInfo localTime, MAUS.Anwendungshandbuch? ahb, bool convertFromUTC = false)
        {
            var format = EdifactFormatHelper.FromPruefidentifikator(pid);
            string jsonBody = null;
            try
            {
                jsonBody = await _loader.LoadJSONTemplate(format, formatPackage.ToLegacyVersionString(), $"{pid}.json");
            }
            catch (Exception)
            {
                //we don't have a mask for this pid, which is fine, go ahead
            }
            var mappingsBody = await _loader.LoadJSONTemplate(format, formatPackage.ToLegacyVersionString(), format + ".json");

            var json = jsonBody != null ? JsonConvert.DeserializeObject<JObject>(jsonBody) : null;
            var inputJson = JsonConvert.DeserializeObject<JObject>(jsonInput);
            var mappings = JsonConvert.DeserializeObject<JArray>(mappingsBody);
            //map inputJson via fix values from mapping
            // if (((JObject)json.Where(entry => ((JObject)entry).Property("key").Value.Value<string>() == "utilmd").FirstOrDefault()) != null)
            //{
            //   ApplyFix(inputJson, ((JObject)json.Where(entry => ((JObject)entry).Property("key").Value.Value<string>() == "utilmd").FirstOrDefault()));
            // }
            //  var processMapping =json;// ((JObject)json.Where(entry => ((JObject)entry).Property("key").Value.Value<string>() == pid).FirstOrDefault());
            //   if (processMapping != null)
            //  {
            //       ApplyFix(inputJson, processMapping);
            //  }
            //   else
            //    {
            //       throw new BadPIDException(pid);
            //    }
            var version = mappings[0]["_meta"]["version"].Value<string>();
            JArray maskArray = null;
            if (json != null)
            {
                maskArray = new JArray();
                foreach (var step in json.Property("steps").Value)
                {
                    foreach (var prop in ((step as JObject)?.Property("fields").Value as JObject).Properties())
                    {
                        var newObj = new JObject
                    {
                        { "key", prop.Name },
                        { "type", (prop.Value as JObject).Property("obligType").Value }
                    };
                        maskArray.Add(newObj);
                    }
                    //maskArray.Merge(((step as JObject)?.Property("fields").Value as JObject).Properties(), new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union });
                }
            }
            var outputJson = CreateMsgJSON(inputJson, mappings, maskArray, ahb?.Lines?.FirstOrDefault(), null, false, new Stack<string>(), out var subParent, convertFromUTC);
            EdiObject result = EdiObject.CreateFromJSON(JsonConvert.SerializeObject(outputJson));
            //apply scripts
            var createTemplate = await _loader.LoadEDITemplate(new EDIFileInfo
            {
                Format = format,
                Version = version,
            }, "create.template");
            return MappingHelper.ExecuteMappings(result, new EDIFileInfo
            {
                Format = format,
                Version = version
            }, new List<string>(), createTemplate, localTime, convertFromUTC);
        }
        public async Task<string> CreateFromEdiJson(string jsonInput, string pid, EdifactFormatVersion formatPackage, TimeZoneInfo localTime, bool convertFromUTC = false)
        {
            var format = EdifactFormatHelper.FromPruefidentifikator(pid);

            var mappingsBody = await _loader.LoadJSONTemplate(format, formatPackage.ToLegacyVersionString(), format + ".json");


            var inputJson = JsonConvert.DeserializeObject<JObject>(jsonInput);
            var mappings = JsonConvert.DeserializeObject<JArray>(mappingsBody);

            var version = mappings[0]["_meta"]["version"].Value<string>();

            EdiObject result = EdiObject.CreateFromJSON(jsonInput);
            //apply scripts
            var createTemplate = await _loader.LoadEDITemplate(new EDIFileInfo
            {
                Format = format,
                Version = version,
            }, "create.template");
            return MappingHelper.ExecuteMappings(result, new EDIFileInfo
            {
                Format = format,
                Version = version,
            }, new List<string>(), createTemplate, localTime, convertFromUTC);
        }
        protected static void ParseObject(JObject value, IDictionary<string, object> target, JArray mappings, bool _)
        {
            if (value is null)
            {
                // instead of running into the NullReferenceException only one line later we can also just throw an Exception here
                throw new ArgumentNullException(nameof(value), "This might be due to a missing tree object. Are you sure that template and tree have been loaded correctly?");
            }
            foreach (var prop in value.Properties())
            {
                var deps = mappings.Where(map => FindDependentObject(map, prop.Name, out var propVal) != null).ToList();
                if (deps.Any())
                {
                    foreach (var dep in deps)
                    {
                        var retObj = FindDependentObject(dep, prop.Name, out var propVal);
                        var superValue = ((JProperty)retObj.First).Value.Value<string>();
                        var superKey = ((JProperty)retObj.First).Name;
                        if (propVal.Type == JTokenType.Array)
                        {
                            dynamic obj = new ExpandoObject();
                            //call recursively
                            var returns = new JArray();
                            if (prop.Value.Type == JTokenType.Array)
                            {
                                obj = new List<dynamic>();
                                foreach (var entry in prop.Value as JArray)
                                {
                                    dynamic subObj = new ExpandoObject();
                                    ParseObject(entry as JObject, subObj as IDictionary<string, object>, (JArray)propVal, _);
                                    obj.Add(subObj);
                                }
                            }
                            else
                            {
                                ParseObject(prop.Value as JObject, obj as IDictionary<string, object>, (JArray)propVal, _);
                            }

                            target.Add(((JObject)dep).Property("key").Value.Value<string>(), obj);

                            continue;
                        }

                        //special case for groupBy fields
                        if (superKey != prop.Name && superValue != prop.Name)
                        {
                            //dynamic obj = new ExpandoObject();
                            if (!target.ContainsKey(superValue))
                            {
                                //create group array
                                target.Add(superValue, new JArray());
                            }
                            var newProp = new ExpandoObject();
                            AddProperty(newProp, ((JValue)propVal).Value<string>(), prop.Value);
                            var addObj = new JObject();
                            //if we already have an object in the array, just add the new property
                            if ((target[superValue] as JArray).Any())
                            {
                                addObj = (target[superValue] as JArray)[0] as JObject;
                            }

                            // go through all new defined properties and add them to the JObject
                            foreach (var (key, o) in newProp)
                            {
                                addObj.Add(key, JToken.FromObject(o));
                            }
                            if ((target[superValue] as JArray).Count == 0)
                            {
                                (target[superValue] as JArray).Add(addObj);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(prop.Value.Value<string>()))
                            {
                                AddProperty(target, ((JValue)propVal).Value<string>(), prop.Value);
                            }
                        }
                    }
                }
            }
        }

        protected static void AddProperty(IDictionary<string, object> target, string name, JToken value)
        {
            while (true)
            {
                if (name.Contains(".") == false)
                {
                    target.Add(name, value);
                }
                else
                {
                    var splits = name.Split(new[] { "." }, StringSplitOptions.None);
                    if (target.ContainsKey(splits.First()))
                    {
                        target = target[splits.First()] as IDictionary<string, object>;
                        name = string.Join(".", splits.Skip(1));
                        continue;
                    }

                    var newObj = new ExpandoObject();
                    target.Add(splits.First(), newObj);
                    target = target[splits.First()] as IDictionary<string, object>;
                    name = string.Join(".", splits.Skip(1));
                    continue;
                }

                break;
            }
        }

        static readonly Regex noLetterRegex = new Regex("[^A-Za-z]", RegexOptions.Compiled);
        protected static bool CompareKey(string left, string right)
        {
            var leftReplaced = noLetterRegex.Replace(left, "");
            var rightReplaced = noLetterRegex.Replace(right, "");
            return leftReplaced == rightReplaced;
        }

        protected static bool FindMask(JArray mask, string maskKey, MAUS.SegmentGroup? ahb, Stack<string> parentPath)
        {
            if (mask == null)
            {
                //if we don't have a mask for this process allow all fields as default
                return true;
            }

            foreach (var maskEntry in mask)
            {
                if (CompareKey((maskEntry as JObject).Property("key")?.Value.Value<string>(), maskKey))
                {
                    if ((maskEntry as JObject).Property("type")?.Value.Value<string>() == "N")
                    {
                        continue;
                    }

                    return (maskEntry as JObject).Property("type")?.Value.Value<string>() != "N";
                }
            }
            return false;
        }
        protected static dynamic CreateMsgJSON(JObject input, JArray mapping, JArray mask, MAUS.SegmentGroup? ahb, List<MAUS.Segment>? segments, bool virtualChild, Stack<string> parentPath, out bool createInParent, bool convertFromUTC = false)
        {
            createInParent = false;
            if (input == null)
            {
                return null;
            }

            var returnObject = new ExpandoObject();
            foreach (var prop in input.Properties())
            {
                MAUS.SegmentGroup? localAhb = ahb;
                var deps = mapping.Where(map => FindObjectByKey(map, prop.Name, out var propVal, false) != null).ToList();
                if (deps.Any())
                {
                    var foundObj = FindObjectByKey(deps.First(), prop.Name, out var propVal, false);
                    if (foundObj is not null)
                    {
                        string sg = foundObj.SelectToken("_meta.sg")?.Value<string>();
                        string key = prop.Name;
                        string virtualKey = foundObj.SelectToken("_meta.virtualKey")?.Value<string>();
                        var rootLikeSgKeys = new HashSet<string>() { "UNH", "/", "root" };
                        if (virtualKey is not null)
                        {
                            key = virtualKey;
                        }
                        if (ahb is not null || localAhb is not null)
                        {
                            if (sg is not null && sg != "/" && sg != "UNH" && sg.StartsWith("SG") && sg != ahb?.Discriminator && virtualKey is null && !virtualChild)
                            {
                                if (localAhb?.SegmentGroups.Any(s => s.Discriminator == sg) == true)
                                {
                                    localAhb = localAhb.SegmentGroups.First(s => s.Discriminator == sg);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if (localAhb?.Segments.Any(s => s.SectionName.Replace(" ", "").Dehumanize() == key.Replace(" ", "").Dehumanize()) == true) // check segments
                            {
                                bool notFoundAnySegment = true;
                                foreach (var segment in localAhb.Segments.Where(s => s.SectionName.Replace(" ", "").Dehumanize() == key.Replace(" ", "").Dehumanize()))
                                {

                                    var id = foundObj.SelectToken("_meta.id")?.Value<string>();
                                    if (id is null)
                                    {
                                        var group = foundObj.SelectToken("_meta.type")?.Value<string>();
                                        if (group == "group")
                                        {
                                            notFoundAnySegment = false;

                                        }
                                    }
                                    else
                                    {
                                        if (segment.DataElements.Any(s => s.DataElementId.Dehumanize() == id.Dehumanize()))
                                        {
                                            notFoundAnySegment = false;
                                        }
                                    }

                                }
                                if (notFoundAnySegment)
                                {
                                    continue;
                                }
                            }
                            else if (segments is not null && segments.Any()) // check segments
                            {
                                var id = foundObj.SelectToken("_meta.id")?.Value<string>();
                                if (id is null)
                                {
                                    var group = foundObj.SelectToken("_meta.type")?.Value<string>();
                                    if (group != "group")
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    bool notFoundAnySegment = true;
                                    foreach (var segment in segments)
                                    {
                                        if (segment.DataElements.Any(s => s.DataElementId.Dehumanize() == id.Dehumanize()))
                                        {
                                            notFoundAnySegment = false;
                                        }
                                    }
                                    if (notFoundAnySegment)
                                    {
                                        continue;
                                    }
                                }
                            }
                            else if (sg is not null && !rootLikeSgKeys.Contains(sg) && sg == ahb?.Discriminator)
                            {
                                continue;
                            }

                        }
                        else if (segments is not null && segments.Any()) // check segments
                        {
                            var id = foundObj.SelectToken("_meta.id")?.Value<string>();
                            if (id is null)
                            {
                                var group = foundObj.SelectToken("_meta.type")?.Value<string>();
                                if (group != "group")
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                bool notFoundAnySegment = true;
                                foreach (var segment in segments)
                                {
                                    if (segment.DataElements.Any(s => s.DataElementId.Dehumanize() == id.Dehumanize()))
                                    {
                                        notFoundAnySegment = false;
                                    }
                                }
                                if (notFoundAnySegment)
                                {
                                    continue;
                                }
                            }
                        }
                        else if (sg is not null && rootLikeSgKeys.Contains(sg) && sg == ahb?.Discriminator)
                        {
                            continue;
                        }
                    }
                    if (propVal == null) // then create new target element and recurse
                    {

                        var newMappings = ((deps.First() as JObject).Property("requires").Value as JArray).Select(req =>
                        {
                            if ((req as JObject).Properties().FirstOrDefault()?.Value as JArray != null)
                            {
                                return (req as JObject).Properties().FirstOrDefault()?.Value as JArray;
                            }

                            return null;
                        }).ToArray();
                        var newArray = new JArray();
                        newArray = newMappings.Where(map => map != null).Aggregate(newArray, (current, map) => new JArray(current.Union(map)));
                        if (newMappings.Any() && !newArray.Any())
                        {
                            //Spezialfall für "groupBy"-Array-Objekte (z.B. Beginn der Nachricht)
                            newArray.Add(deps.First());
                            // Das Array kann in einem groupBy-Fall immer nur ein Element haben
                            // war früher ein JObject, daher hier die Ausnahme abfangen
                            if (prop.Value.GetType() == typeof(JObject))
                            {
                                continue;
                            }
                            parentPath.Push(prop.Name);
                            var newSub = CreateMsgJSON((prop.Value as JArray)[0] as JObject, newArray, mask, ahb, null, false, parentPath, out var subParent, convertFromUTC);
                            foreach (var (key, value) in newSub as IDictionary<string, object>)
                            {
                                (returnObject as IDictionary<string, object>).Add(key, value);
                            }

                            continue;
                        }
                        if (!newArray.Any())
                        {
                            var subObj = FindObjectByKey(deps.First(), prop.Name, out propVal, false);
                            if (subObj.SelectToken("_meta") != null)
                            {
                                var format = subObj.SelectToken("_meta.format").Value<string>();
                                //format date
                                (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), new ScriptHelper { useLocalTime = convertFromUTC }.FormatDate(prop.Value.Value<string>(), format));
                            }
                            else
                            {
                                (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), prop.Value);
                            }
                        }
                        else
                        {
                            //check for virtual groups
                            var virtualGroup = false;
                            var subObj = FindObjectByKey(deps.First(), prop.Name, out propVal, false);
                            if (subObj.SelectToken("_meta.virtual") != null)
                            {
                                virtualGroup = subObj.SelectToken("_meta.virtual").Value<bool>();
                            }

                            if (FindMask(mask, prop.Name, ahb, parentPath) || prop.Name == "Dokument" || prop.Name == "Nachricht" || virtualGroup)
                            {
                                var newPropName = ((deps.First() as JObject).Property("requires").Value.Value<JArray>().FirstOrDefault() as JObject)?.Properties().FirstOrDefault()?.Name;
                                if (prop.Value.Type == JTokenType.Array)
                                {
                                    (returnObject as IDictionary<string, object>).Add(newPropName, new List<dynamic>());
                                    int index = 0;
                                    foreach (var sub in prop.Value as JArray)
                                    {
                                        if (sub is JObject)
                                        {
                                            parentPath.Push($"{prop.Name}[{index}]");
                                            List<MAUS.Segment?> localSegment = new List<MAUS.Segment?>();
                                            bool isVirtualKey = false;
                                            if (localAhb is not null)
                                            {
                                                var key = prop.Name;
                                                var virtualKey = foundObj.SelectToken("_meta.virtualKey")?.Value<string>();
                                                if (virtualKey is not null)
                                                {
                                                    key = virtualKey;
                                                    isVirtualKey = true;
                                                }
                                                if (localAhb?.Segments?.Any(s => s.SectionName.Replace(" ", "").Dehumanize() == key.Replace(" ", "").Dehumanize()) == true)
                                                {
                                                    foreach (var segment in localAhb?.Segments?.Where(s => s.SectionName.Replace(" ", "").Dehumanize() == key.Replace(" ", "").Dehumanize()))
                                                    {
                                                        if (segment.Discriminator == "CCI") //special handling for CCI/CAV
                                                        {
                                                            var seg_index = localAhb.Segments.IndexOf(segment);
                                                            do
                                                            {
                                                                seg_index++;
                                                                try
                                                                {
                                                                    if (localAhb.Segments[seg_index].Discriminator != "CAV")
                                                                    {
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (!localSegment.Contains(localAhb.Segments[seg_index]))
                                                                        {
                                                                            localSegment.Add(localAhb.Segments[seg_index]);
                                                                        }
                                                                    }
                                                                }
                                                                catch (ArgumentOutOfRangeException)
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            while (true);

                                                        }
                                                        if (!localSegment.Contains(segment))
                                                        {
                                                            localSegment.Add(segment);
                                                        }
                                                    }
                                                }
                                            }
                                            else if (localAhb?.Discriminator != "root")
                                            {

                                            }
                                            if (localAhb is not null && localAhb?.Discriminator != "root" && !localSegment.Any())
                                            {
                                                continue;
                                            }

                                            var newSub = CreateMsgJSON(sub as JObject, newArray, mask, localAhb, localSegment, virtualChild || isVirtualKey, parentPath, out var subParent, convertFromUTC);
                                            if (!subParent)
                                            {
                                                ((returnObject as IDictionary<string, object>)[newPropName] as List<dynamic>).Add(newSub);
                                            }
                                            else
                                            {
                                                dynamic newObj = new ExpandoObject();
                                                foreach (var (key, value) in (newSub as IDictionary<string, object>).ToList())
                                                {
                                                    (newObj as IDictionary<string, object>).Add(key, value);
                                                }
                                                ((returnObject as IDictionary<string, object>)[newPropName] as List<dynamic>).Add(newObj);
                                            }
                                        }
                                        index++;
                                    }
                                }
                                else
                                {
                                    parentPath.Push(prop.Name);
                                    var newSub = CreateMsgJSON(prop.Value as JObject, newArray, mask, localAhb, null, false, parentPath, out var subParent, convertFromUTC);
                                    if (!subParent)
                                    {
                                        (returnObject as IDictionary<string, object>).Add(newPropName, newSub);
                                    }
                                    else
                                    {
                                        dynamic newObj = new ExpandoObject();
                                        foreach (var (key, value) in (newSub as IDictionary<string, object>).ToList())
                                        {
                                            (newObj as IDictionary<string, object>).Add(key, value);
                                        }
                                        ((returnObject as IDictionary<string, object>)[newPropName] as List<dynamic>).Add(newObj);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (foundObj.Property("key") != null)
                        {
                            //check for validity mask
                            if (!FindMask(mask, foundObj.Property("key").Value.Value<string>(), ahb, parentPath))
                            {
                                continue;
                            }
                        }

                        switch (prop.Value.Type)
                        {
                            //if we have a complex type (e.g. Absender.Code) make sure to apply all values
                            case JTokenType.Array:
                                {
                                    foreach (var dep in deps)
                                    {
                                        FindObjectByKey(dep, prop.Name, out var newVal, false);
                                        var valPath = dep[newVal.Value<string>()].Value<string>();
                                        var pathParts = valPath.Split('.');
                                        //TODO: generalize this to enable more deep object nesting (e.g. A.B.C)
                                        createInParent = true;
                                        (returnObject as IDictionary<string, object>).Add(newVal.Value<string>(), prop.Value[0][pathParts[1]]);

                                    }

                                    break;
                                }
                            case JTokenType.Object:
                                {
                                    foreach (var dep in deps)
                                    {
                                        FindObjectByKey(dep, prop.Name, out var newVal, false);
                                        var valPath = dep[newVal.Value<string>()].Value<string>();
                                        var pathParts = valPath.Split('.');
                                        if (pathParts.Length > 1)
                                        {
                                            //TODO: generalize this to enable more deep object nesting (e.g. A.B.C)
                                            createInParent = true;
                                            (returnObject as IDictionary<string, object>).Add(newVal.Value<string>(), prop.Value[pathParts[1]]);
                                        }
                                        else
                                        {
                                            (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), prop.Value);
                                        }

                                    }

                                    break;
                                }
                            default:
                                (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), prop.Value);
                                break;
                        }
                    }

                }
            }
            return returnObject;
        }
        protected static void SetValue(JObject input, string path, string value)
        {
            var splits = path.Split(new[] { "[]." }, StringSplitOptions.None);
            if (input.SelectToken(splits.First()) is JArray)
            {
                foreach (var subObj in (JArray)input.SelectToken(splits.First()))
                {
                    SetValue(subObj as JObject, string.Join("[].", splits.Skip(1)), value);
                }
            }
            else if (input.SelectToken(splits.First()) != null)
            {
                //do nothing as the source already contains this value
            }
            else
            {
                if (splits.Length > 1)
                {
                    //add array
                    var newArray = new JArray
                    {
                        new JObject()
                    };
                    SetValue(newArray.First as JObject, string.Join("[].", splits.Skip(1)), value);
                    //unescape name
                    var unescapedName = splits.First().Replace("['", "").Replace("']", "");
                    input.Add(unescapedName, newArray);
                }
                else
                {
                    //add property
                    //unescape name
                    var unescapedName = splits.First().Replace("['", "").Replace("']", "");
                    input.Add(unescapedName, value);
                }
            }
        }
        protected static JObject ApplyFix(JObject input, JObject fix)
        {
            if (fix.SelectToken("fix") == null)
            {
                return null;
            }

            foreach (JObject subFix in fix.SelectToken("fix") as JArray)
            {
                var name = subFix.SelectToken("_name").Value<string>();
                var value = "";
                if (subFix.SelectToken("_value") != null)
                {
                    value = subFix.SelectToken("_value").Value<string>();
                }
                else if (subFix.SelectToken("_path") != null)
                {
                    value = input.SelectToken(subFix.SelectToken("_path")?.Value<string>())?.Value<string>();
                }
                if (name.Contains("[]."))
                {
                    SetValue(input, name, value);
                }
                else
                {
                    (input.SelectToken(name) as JProperty).Value = value;
                }
            }
            return input;
        }
        protected static JObject FindDependentObject(JToken val, string key, out JToken propVal)
        {
            propVal = null;

            if (val == null)
            {
                return null;
            }

            if (val.Type != JTokenType.Object)
            {
                return null;
            }

            var obj = (JObject)val;
            var requires = obj.SelectToken("requires");
            if (requires == null || requires.Type != JTokenType.Array)
            {
                if (obj.Property(key) != null)
                {
                    propVal = obj.Property(key).Value;
                    return obj;
                }

                return null;
            }
            foreach (var req in (JArray)requires)
            {
                if (req.Type != JTokenType.Object)
                {
                    continue;
                }

                var reqObj = (JObject)req;
                if (reqObj.Properties().FirstOrDefault()?.Name == key)
                {
                    propVal = reqObj.Properties().First().Value;
                    return obj;
                }
            }
            return null;
        }
        protected static bool CheckFieldName(string name, string key)
        {
            if (name == key)
            {
                return true;
            }

            if (name.Contains("."))
            {
                if (name.Split('.').First() == key)
                {
                    return true;
                }
            }
            return false;
        }
        protected static JObject FindObjectByKey(JToken val, string key, out JToken propVal, bool ignoreParentKey)
        {
            //searches in a mapping json object for a corresponding key and returns the found token
            propVal = null;

            //if we don't provide a mapping (variable val) or it is not an object return
            if (val == null)
            {
                return null;
            }

            if (val.Type != JTokenType.Object)
            {
                return null;
            }

            var obj = (JObject)val;
            //we have found a top level match (val.key), check if we want top level matches and return
            if (!ignoreParentKey && obj.Property("key")?.Value.Value<string>() == key)
            {
                return obj;
            }
            //now go deeper and look for object names in the requires array
            var requires = obj.SelectToken("requires");
            //if requires is an object and not an array look for matching keys in the object properties
            if (requires == null || requires.Type != JTokenType.Array)
            {
                foreach (var prop in obj.Properties())
                {
                    if (!prop.Name.StartsWith("_") && prop.Value.Type == JTokenType.String)
                    {
                        if (CheckFieldName(prop.Value.Value<string>(), key))
                        {
                            propVal = prop.Name;
                            return obj;
                        }
                    }
                }
                return null;

            }
            //if it is an array check all entries in the array
            foreach (JObject req in (JArray)requires)
            {
                // check all properties for a match
                foreach (var sub in req.Properties())
                {
                    //only check properties with value type string (otherwise we can't compare the key name)
                    if (sub.Value.Type == JTokenType.String)
                    {
                        if (CheckFieldName(sub.Value.Value<string>(), key))
                        {
                            propVal = sub.Name;
                            return req;
                        }
                    }
                }
            }
            return null;
        }
    }
}
