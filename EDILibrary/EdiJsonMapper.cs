// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using EDILibrary.Exceptions;
using EDILibrary.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EDILibrary
{
    public class EdiJsonMapper
    {
        protected Interfaces.ITemplateLoader _loader;

        public EdiJsonMapper(Interfaces.ITemplateLoader loader)
        {
            _loader = loader;
        }

        public async Task<string> ParseToJson(string edi, string packageVersion, string includeEmptyValues = null)
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
            //mapping laden
            JArray mappings = null;
            try
            {
                mappings = JsonConvert.DeserializeObject<JArray>(await _loader.LoadJSONTemplate(package, edi_info.Format + edi_info.Version + ".json"));

            }
            catch(Exception e)
            {
                throw new BadFormatException(edi_info.Format, edi_info.Version);
            }
            dynamic resultObject = new ExpandoObject();

            var resultDict = resultObject as IDictionary<string, object>;
            ParseObject(jsonResult, resultObject as IDictionary<string, object>, mappings, includeEmptyValues != null);

            return JsonConvert.SerializeObject(resultObject);
        }
        public async Task<string> CreateFromJson(string jsonInput, string pid, string formatPackage = null)
        {
            string format = "UTILMD";
            string version = "5.1g";
            string package = null;
            if (formatPackage != null)
            {
                package = formatPackage;
            }
            else
            {
                throw new Exception("FormatPackage must be specified");
            }
            var json = JsonConvert.DeserializeObject<JArray>(await _loader.LoadJSONTemplate(package, "wim_utilmd.json"));
            var inputJson = JsonConvert.DeserializeObject<JObject>(jsonInput);
            JArray mappings = JsonConvert.DeserializeObject<JArray>(await _loader.LoadJSONTemplate(package, format + version + ".json"));
            //map inputJson via fix values from mapping
            if (((JObject)json.Where(entry => ((JObject)entry).Property("key").Value.Value<string>() == "utilmd").FirstOrDefault()) != null)
            {
                ApplyFix(inputJson, ((JObject)json.Where(entry => ((JObject)entry).Property("key").Value.Value<string>() == "utilmd").FirstOrDefault()));
            }
            var processMapping = ((JObject)json.Where(entry => ((JObject)entry).Property("key").Value.Value<string>() == pid).FirstOrDefault());
            if (processMapping != null)
            {
                ApplyFix(inputJson, processMapping);
            }
            else
            {
                throw new BadPIDException(pid);
            }
            JArray maskArray = new JArray();
            foreach (var step in processMapping.Property("steps").Value)
            {
                maskArray.Merge((step as JObject)?.Property("fields").Value as JArray, new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union });
            }
            var outputJson = CreateMsgJSON(inputJson, mappings, maskArray);
            IEdiObject result = IEdiObject.CreateFromJSON(JsonConvert.SerializeObject(outputJson));
            //apply scripts
            return await new MappingHelper().ExecuteMappings(result, new EDIFileInfo() { Format = format, Version = version }, new List<string>(), _loader);

        }
        protected void ParseObject(JObject value, IDictionary<string, object> target, JArray mappings, bool includeEmptyValues)
        {
            foreach (var prop in value.Properties())
            {
                var deps = mappings.Where(map => FindDependentObject(map, prop.Name, out JToken propVal) != null).ToList();
                if (deps.Count() > 0)
                {
                    var retObj = FindDependentObject(deps.First(), prop.Name, out JToken propVal);
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
                                ParseObject(entry as JObject, subObj as IDictionary<string, object>, ((JArray)propVal), includeEmptyValues);
                                obj.Add(subObj);
                            }
                        }
                        else
                        {
                            ParseObject(prop.Value as JObject, obj as IDictionary<string, object>, ((JArray)propVal), includeEmptyValues);
                        }

                        target.Add(((JObject)deps.First()).Property("key").Value.Value<string>(), obj);

                        continue;
                    }
                    else
                    {
                        if (superKey != prop.Name && superValue != prop.Name)
                        {
                            dynamic obj = new ExpandoObject();
                            if (!target.ContainsKey(superValue))
                            {
                                target.Add(superValue, new JArray(new ExpandoObject()));
                            }
                            AddProperty((((target[superValue])as JArray)[0] as IDictionary<string, object>), ((JValue)propVal).Value<string>(), prop.Value);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(prop.Value.Value<string>()))
                                AddProperty(target, ((JValue)propVal).Value<string>(), prop.Value);
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
                    AddProperty(target[splits.First()] as IDictionary<string, object>, String.Join(".", splits.Skip(1)), value);
                }
                else
                {
                    var newObj = new ExpandoObject();
                    target.Add(splits.First(), newObj);
                    AddProperty(target[splits.First()] as IDictionary<string, object>, String.Join(".", splits.Skip(1)), value);
                }
            }
        }
        protected bool FindMask(JArray mask, string maskKey)
        {
            if (mask == null)
                return false;
            foreach (var maskEntry in mask)
            {
                if ((maskEntry as JObject).Property("key")?.Value.Value<string>() == maskKey)
                    return (maskEntry as JObject).Property("type")?.Value.Value<string>() == "M";
            }
            return false;
        }
        protected dynamic CreateMsgJSON(JObject input, JArray mapping, JArray mask)
        {
            var returnObject = new ExpandoObject();
            foreach (var prop in input.Properties())
            {
                var deps = mapping.Where(map => FindObjectByKey(map, prop.Name, out JToken propVal, false) != null).ToList();
                if (deps.Count() > 0)
                {
                    var foundObj = FindObjectByKey(deps.First(), prop.Name, out JToken propVal, false);
                    if (propVal == null) // then create new target element and recurse
                    {

                        var newMappings = (((deps.First() as JObject).Property("requires").Value as JArray).Select(req =>
                        {
                            if ((((req as JObject).Properties().FirstOrDefault() as JProperty)?.Value as JArray) != null)
                                return (((req as JObject).Properties().FirstOrDefault() as JProperty)?.Value as JArray);
                            else
                                return null;
                        })).ToArray();
                        JArray newArray = new JArray();
                        foreach (var map in newMappings)
                        {
                            if (map != null)
                                newArray = new JArray(newArray.Union(map));
                        }
                        if (newMappings.Count() > 0 && newArray.Count == 0)
                        {
                            //Spezialfall für "groupBy"-Objekte (z.B. Beginn der Nachricht)
                            newArray.Add(deps.First());
                            var newSub = CreateMsgJSON(prop.Value as JObject, newArray, mask);
                            foreach (KeyValuePair<string,object> subProp in (newSub as IDictionary<string,object>))
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
                                (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), new ScriptHelper().FormatDate(prop.Value.Value<string>(), format));
                            }
                            else
                            {
                                (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), prop.Value);
                            }
                        }
                        else
                        {
                            if (FindMask(mask, prop.Name) || prop.Name == "Dokument" || prop.Name == "Nachricht")
                            {
                                var newPropName = ((deps.First() as JObject).Property("requires").Value.Value<JArray>().FirstOrDefault() as JObject)?.Properties().FirstOrDefault()?.Name;
                                if (prop.Value.Type == JTokenType.Array)
                                {
                                    (returnObject as IDictionary<string, object>).Add(newPropName, new List<dynamic>());
                                    foreach (var sub in prop.Value as JArray)
                                    {
                                        var newSub = CreateMsgJSON(sub as JObject, newArray, mask);
                                        ((returnObject as IDictionary<string, object>)[newPropName] as List<dynamic>).Add(newSub);
                                    }
                                }
                                else
                                {
                                    var newSub = CreateMsgJSON(prop.Value as JObject, newArray, mask);
                                    (returnObject as IDictionary<string, object>).Add(newPropName, newSub);
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
                        (returnObject as IDictionary<string, object>).Add(propVal.Value<string>(), prop.Value);
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
                    SetValue(subObj as JObject, String.Join("[].", splits.Skip(1)), value);
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
                    SetValue(newArray.First as JObject, String.Join("[].", splits.Skip(1)), value);
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
        protected JObject FindObjectByKey(JToken val, string key, out JToken propVal, bool ignoreParentKey)
        {

            propVal = null;

            if (val == null)
                return null;
            if (val.Type != JTokenType.Object)
                return null;
            JObject obj = (JObject)val;
            if (!ignoreParentKey && obj.Property("key")?.Value.Value<string>() == key)
            {
                return obj;
            }
            var requires = obj.SelectToken("requires");
            if (requires == null || requires.Type != JTokenType.Array)
            {
                foreach (JProperty prop in obj.Properties())
                {
                    if (!prop.Name.StartsWith("_") && prop.Value.Value<string>() == key)
                    {
                        propVal = prop.Name;
                        return obj;
                    }
                }
                return null;

            }
            foreach (JObject req in (JArray)requires)
            {
                if (req.Type != JTokenType.Object)
                {
                    //if (((JProperty)req).Value.Value<string>() == key)
                    //{
                    //    propVal = ((JProperty)req).Name;
                    //}
                }

                foreach (JProperty sub in req.Properties())
                {
                    if (sub.Value.Type == JTokenType.String)
                    {
                        if (sub.Value.Value<string>() == key)
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
