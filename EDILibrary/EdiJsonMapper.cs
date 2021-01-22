// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using EDILibrary.Exceptions;
using EDILibrary.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;

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
            public string Format;
            public string Version;
            public string Sender;
            public string Receiver;
        }
        public async Task<string> ParseToJson(string edi, string packageVersion, string includeEmptyValues = null)
        {
            return (await ParseToJsonWithVersion(edi, packageVersion, includeEmptyValues)).EDI;
        }
            public async Task<JsonResult> ParseToJsonWithVersion(string edi, string packageVersion, string includeEmptyValues = null)
        {
            var edi_info = EDIHelper.GetEDIFileInfo(edi);
            var edi_string = EDIHelper.NormalizeEDIHeader(edi);
            string treeString = await _loader.LoadEDITemplate(edi_info, "tree");
            string templateString = await _loader.LoadEDITemplate(edi_info, "template");
            EDILibrary.GenericEDILoader loader = new GenericEDILoader();
            XElement template = loader.LoadTemplate(templateString);
            TreeElement tree = loader.LoadTree(treeString);
            var edi_tree = loader.LoadEDI(edi_string, tree);
            TreeHelper.RefreshDirtyFlags(tree);
            var fileObject = loader.LoadTemplateWithLoadedTree(template, edi_tree);
            var jsonResult = JsonConvert.DeserializeObject<JObject>(fileObject.SerializeToJSON());
            string package = null;
            if (packageVersion != null)
            {
                package = packageVersion;
            }
            else
            {
                //The template loader can try to read the package from the format and version (and the right table)
                package = edi_info.Format + "|" + edi_info.Version;          
            }
            //mapping laden
            JArray mappings = null;
            try
            {
                mappings = JsonConvert.DeserializeObject<JArray>(await _loader.LoadJSONTemplate(package, edi_info.Format + ".json"));
            }
            catch (Exception e)
            {
                throw new BadFormatException(edi_info.Format, edi_info.Version);
            }
            dynamic resultObject = new ExpandoObject();

            var resultDict = resultObject as IDictionary<string, object>;
            ParseObject(jsonResult, resultObject as IDictionary<string, object>, mappings, includeEmptyValues != null);
            JsonResult result = new JsonResult();
            result.EDI = JsonConvert.SerializeObject(resultObject);
            result.Format = edi_info.Format;
            result.Version = edi_info.Version;
            result.Sender = edi_info.Sender.ID;
            result.Receiver = edi_info.Empfänger.ID;
            return result;
        }
        public async Task<string> CreateFromJson(string jsonInput, string pid, string formatPackage = null)
        {
            string format = "ERROR";
            //format is derived from the pid
            switch (pid.Substring(0, 2))
            {
                case "11": format = "UTILMD"; break;
                case "13": format = "MSCONS"; break;
                case "17": format = "ORDERS"; break;
                case "19": format = "ORDRSP"; break;
                case "21": format = "IFTSTA"; break;
                case "70": format = "SSQNOT"; break;
                case "31": format = "INVOIC"; break;
                case "33": format = "REMADV"; break;
                case "27": format = "PRICAT"; break;
                case "35": format = "REQOTE"; break;
                case "15": format = "QUOTES"; break;
                case "23": format = "INSRPT"; break;
                case "99": format = "APERAK"; break;
            }
            string package = null;
            if (formatPackage != null)
            {
                package = formatPackage;
            }
            else
            {
                throw new Exception("FormatPackage must be specified");
            }
            var json = JsonConvert.DeserializeObject<JObject>(await _loader.LoadJSONTemplate(package, $"{pid}.json"));
            var inputJson = JsonConvert.DeserializeObject<JObject>(jsonInput);
            JArray mappings = JsonConvert.DeserializeObject<JArray>(await _loader.LoadJSONTemplate(package, format + ".json"));
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
            string version = mappings[0]["_meta"]["version"].Value<string>();

            JArray maskArray = new JArray();
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
            var outputJson = CreateMsgJSON(inputJson, mappings, maskArray, out var subParent);
            IEdiObject result = IEdiObject.CreateFromJSON(JsonConvert.SerializeObject(outputJson));
            //apply scripts
            return await new MappingHelper().ExecuteMappings(result, new EDIFileInfo() { Format = format, Version = version }, new List<string>(), _loader,false);

        }   
        protected void ParseObject(JObject value, IDictionary<string, object> target, JArray mappings, bool includeEmptyValues)
        {
            foreach (var prop in value.Properties())
            {
                var deps = mappings.Where(map => FindDependentObject(map, prop.Name, out JToken propVal) != null).ToList();
                if (deps.Count() > 0)
                {
                    foreach(var dep in deps) {
                        var retObj = FindDependentObject(dep, prop.Name, out JToken propVal);
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
                                    ParseObject(entry as JObject, subObj as IDictionary<string, object>, (JArray)propVal, includeEmptyValues);
                                    obj.Add(subObj);
                                }
                            }
                            else
                            {
                                ParseObject(prop.Value as JObject, obj as IDictionary<string, object>, (JArray)propVal, includeEmptyValues);
                            }

                            target.Add(((JObject)dep).Property("key").Value.Value<string>(), obj);

                            continue;
                        }
                        else
                        {
                            //special case for groupBy fields
                            if (superKey != prop.Name && superValue != prop.Name)
                            {
                                dynamic obj = new ExpandoObject();
                                if (!target.ContainsKey(superValue))
                                {
                                    //create group array
                                    target.Add(superValue, new JArray());
                                }
                                var newProp = new ExpandoObject();
                                AddProperty(newProp, ((JValue)propVal).Value<string>(), prop.Value);
                                var addObj = new JObject();
                                //if we already have an object in the array, just add the new property
                                if ((target[superValue] as JArray).Count > 0)
                                    addObj = (target[superValue] as JArray)[0] as JObject;
                                // go through all new defined properties and add them to the JObject
                                foreach (var kvp in newProp as IDictionary<string, object>)
                                {
                                    addObj.Add(kvp.Key, JToken.FromObject(kvp.Value));
                                }
                                if ((target[superValue] as JArray).Count == 0)
                                    (target[superValue] as JArray).Add(addObj);


                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(prop.Value.Value<string>()))
                                    AddProperty(target, ((JValue)propVal).Value<string>(), prop.Value);
                            }
                        }
                    }
                }
            }
        }
        protected void AddProperty(IDictionary<string, object> target, string name, JToken value)
        {
            if (name.Contains(".") == false)
            {
                target.Add(name, value);
            }
            else
            {
                var splits = name.Split(new string[] { "." }, StringSplitOptions.None);
                if (target.ContainsKey(splits.First()))
                {
                    AddProperty(target[splits.First()] as IDictionary<string, object>, string.Join(".", splits.Skip(1)), value);
                }
                else
                {
                    var newObj = new ExpandoObject();
                    target.Add(splits.First(), newObj);
                    AddProperty(target[splits.First()] as IDictionary<string, object>, string.Join(".", splits.Skip(1)), value);
                }
            }
        }
        protected bool CompareKey(string left, string right)
        {
            Regex rx = new Regex("[^A-Za-z]");
            var left_replaced = rx.Replace(left, "");
            var right_replaced = rx.Replace(right, "");
            return left_replaced == right_replaced;
        }
        protected bool FindMask(JArray mask, string maskKey)
        {
            if (mask == null)
                return false;
            foreach (var maskEntry in mask)
            {
                if (CompareKey((maskEntry as JObject).Property("key")?.Value.Value<string>(), maskKey))
                {
                    if ((maskEntry as JObject).Property("type")?.Value.Value<string>() == "N")
                        continue;
                    return (maskEntry as JObject).Property("type")?.Value.Value<string>() != "N";
                }
            }
            return false;
        }
        protected dynamic CreateMsgJSON(JObject input, JArray mapping, JArray mask, out bool createInParent)
        {
            
            createInParent = false;
            if (input == null)
                return null;
            var returnObject = new ExpandoObject();
            foreach (var prop in input.Properties())
            {
                var deps = mapping.Where(map => FindObjectByKey(map, prop.Name, out JToken propVal, false) != null).ToList();
                if (deps.Count() > 0)
                {
                    var foundObj = FindObjectByKey(deps.First(), prop.Name, out JToken propVal, false);
                    if (propVal == null) // then create new target element and recurse
                    {

                        var newMappings = ((deps.First() as JObject).Property("requires").Value as JArray).Select(req =>
                        {
                            if (((req as JObject).Properties().FirstOrDefault() as JProperty)?.Value as JArray != null)
                                return ((req as JObject).Properties().FirstOrDefault() as JProperty)?.Value as JArray;
                            else
                                return null;
                        }).ToArray();
                        JArray newArray = new JArray();
                        foreach (var map in newMappings)
                        {
                            if (map != null)
                                newArray = new JArray(newArray.Union(map));
                        }
                        if (newMappings.Count() > 0 && newArray.Count == 0)
                        {
                            //Spezialfall für "groupBy"-Array-Objekte (z.B. Beginn der Nachricht)
                            newArray.Add(deps.First());
                            // Das Array kann in einem groupBy-Fall immer nur ein Element haben
                            // war früher ein JObject, daher hier die Ausnahme abfangen
                            if (prop.Value.GetType() == typeof(JObject))
                                continue;
                            var newSub = CreateMsgJSON((prop.Value as JArray)[0] as JObject, newArray, mask, out var subParent);
                            foreach (KeyValuePair<string, object> subProp in newSub as IDictionary<string, object>)
                                (returnObject as IDictionary<string, object>).Add(subProp.Key, subProp.Value);
                            continue;
                        }
                        if (newArray.Count == 0)
                        {
                            var subObj = FindObjectByKey(deps.First(), prop.Name, out propVal, false);
                            if (subObj.SelectToken("_meta") != null)
                            {
                                string format = subObj.SelectToken("_meta.format").Value<string>();
                                //format date
                                (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), new ScriptHelper() { useLocalTime = false }.FormatDate(prop.Value.Value<string>(), format));
                            }
                            else
                            {
                                (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), prop.Value);
                            }
                        }
                        else
                        {
                            //check for virtual groups
                            bool virtualGroup = false;
                            var subObj = FindObjectByKey(deps.First(), prop.Name, out propVal, false);
                            if (subObj.SelectToken("_meta.virtual") != null)
                                virtualGroup = subObj.SelectToken("_meta.virtual").Value<bool>();
                            if (FindMask(mask, prop.Name) || prop.Name == "Dokument" || prop.Name == "Nachricht" || virtualGroup)
                            {
                                var newPropName = ((deps.First() as JObject).Property("requires").Value.Value<JArray>().FirstOrDefault() as JObject)?.Properties().FirstOrDefault()?.Name;
                                if (prop.Value.Type == JTokenType.Array)
                                {
                                    (returnObject as IDictionary<string, object>).Add(newPropName, new List<dynamic>());
                                    foreach (var sub in prop.Value as JArray)
                                    {
                                        if (sub as JObject != null)
                                        {
                                            var newSub = CreateMsgJSON(sub as JObject, newArray, mask, out var subParent);
                                            if (!subParent)
                                                ((returnObject as IDictionary<string, object>)[newPropName] as List<dynamic>).Add(newSub);
                                            else
                                            {
                                                dynamic newObj = new ExpandoObject();
                                                foreach (var newProp in (newSub as IDictionary<string, object>).ToList<KeyValuePair<string, object>>())
                                                {
                                                    (newObj as IDictionary<string, object>).Add(newProp.Key, newProp.Value);
                                                }
                                                ((returnObject as IDictionary<string, object>)[newPropName] as List<dynamic>).Add(newObj);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var newSub = CreateMsgJSON(prop.Value as JObject, newArray, mask, out var subParent);
                                    if (!subParent)
                                        (returnObject as IDictionary<string, object>).Add(newPropName, newSub);
                                    else
                                    {
                                        dynamic newObj = new ExpandoObject();
                                        foreach (var newProp in (newSub as IDictionary<string, object>).ToList<KeyValuePair<string, object>>())
                                        {
                                            (newObj as IDictionary<string, object>).Add(newProp.Key, newProp.Value);
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
                            if (!FindMask(mask, foundObj.Property("key").Value.Value<string>()))
                                continue;
                        }
                        //if we have a complex type (e.g. Absender.Code) make sure to apply all values
                        if (prop.Value.Type == JTokenType.Array)
                        {
                            foreach (var dep in deps)
                            {
                                FindObjectByKey(dep, prop.Name, out JToken newVal, false);
                                var valPath = dep[newVal.Value<string>()].Value<string>();
                                var pathParts = valPath.Split('.');
                                //TODO: generalize this to enable more deep object nesting (e.g. A.B.C)
                                createInParent = true;
                                (returnObject as IDictionary<string, object>).Add(newVal.Value<string>(), prop.Value[0][pathParts[1]]);

                            }
                        }
                        else if (prop.Value.Type == JTokenType.Object)
                        {
                            foreach (var dep in deps)
                            {
                                FindObjectByKey(dep, prop.Name, out JToken newVal, false);
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
                        }
                        else
                        {
                            (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), prop.Value);
                        }
                    }

                }
            }
            return returnObject;
        }
        protected void SetValue(JObject input, string path, string value)
        {
            var splits = path.Split(new string[] { "[]." }, StringSplitOptions.None);
            if (input.SelectToken(splits.First()) as JArray != null)
            {
                foreach (var subObj in input.SelectToken(splits.First()) as JArray)
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
                if (splits.Count() > 1)
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
        protected JObject ApplyFix(JObject input, JObject fix)
        {
            if (fix.SelectToken("fix") == null)
                return null;
            foreach (JObject subFix in fix.SelectToken("fix") as JArray)
            {
                string name = subFix.SelectToken("_name").Value<string>();
                string value = "";
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
        protected JObject FindDependentObject(JToken val, string key, out JToken propVal)
        {

            propVal = null;

            if (val == null)
                return null;
            if (val.Type != JTokenType.Object)
                return null;
            JObject obj = (JObject)val;
            var requires = obj.SelectToken("requires");
            if (requires == null || requires.Type != JTokenType.Array)
            {
                if (obj.Property(key) != null)
                {
                    propVal = obj.Property(key).Value;
                    return obj;
                }
                else
                    return null;
            }
            foreach (var req in (JArray)requires)
            {
                if (req.Type != JTokenType.Object)
                    continue;
                JObject reqObj = (JObject)req;
                if (reqObj.Properties().FirstOrDefault()?.Name == key)
                {
                    propVal = reqObj.Properties().First().Value;
                    return obj;
                }
            }
            return null;
        }
        protected bool CheckFieldName(string name, string key)
        {
            if (name == key)
                return true;
            if (name.Contains("."))
            {
                if (name.Split('.').First() == key)
                    return true;
            }
            return false;
        }
        protected JObject FindObjectByKey(JToken val, string key, out JToken propVal, bool ignoreParentKey)
        {
            //searches in a mapping json object for a corresponding key and returns the found token
            propVal = null;

            //if we don't provide a mapping (variable val) or it is not an object return
            if (val == null)
                return null;
            if (val.Type != JTokenType.Object)
                return null;
            JObject obj = (JObject)val;
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
                foreach (JProperty prop in obj.Properties())
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
                foreach (JProperty sub in req.Properties())
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
