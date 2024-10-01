using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDILibrary
{
    public class TemplateHelper
    {
        private static readonly Regex FormatVersionRegex =
            new(@"^(?<format>[A-Z]{6,7})(?<version>[A-Z]?\d+\.\d+[a-z]?)$");

        private static void ParseAPERAKString(
            string aperak,
            out string dataType,
            out int length,
            out List<string> list
        )
        {
            dataType = null;
            list = null;
            length = 0;
            if (aperak == "N")
            {
                return;
            }

            dataType = aperak.Substring(1, 2); //data type
            var maxLength = aperak.Length;
            var valueIndex = aperak.IndexOf('{');
            if (valueIndex > -1)
            {
                maxLength = valueIndex;
                var valueListString = aperak.Substring(
                    valueIndex + 1,
                    aperak.IndexOf('}', valueIndex) - valueIndex - 1
                );
                list = new List<string>(valueListString.Split(".".ToCharArray()));
            }

            try
            {
                length = int.Parse(aperak.Substring(3, maxLength - 3));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Could not parse {0}", aperak), e);
            }
        }

        private static void Recurse(XElement cur, JArray refObj, TreeElement tree)
        {
            var i = cur.Descendants("field").Count(d => d.Parent == cur);
            var hasClass = cur.Descendants("class").Any(d => d.Parent == cur);
            foreach (var elem in cur.Descendants("field").Where(d => d.Parent == cur))
            {
                //Fields mappen
                var newField = new JObject();
                var name = elem.Attribute("name").Value;
                if (elem.Attribute("ahbName") != null)
                {
                    name = elem.Attribute("ahbName").Value;
                }

                newField.Add(elem.Attribute("name").Value, name);
                // newField.Add("key", name);
                var meta = new JObject();
                if (elem.Attribute("meta.id") != null)
                {
                    meta.Add("id", elem.Attribute("meta.id").Value);
                }

                if (elem.Attribute("meta.help") != null)
                {
                    meta.Add("help", elem.Attribute("meta.help").Value);
                }

                if (elem.Attribute("meta.format") != null)
                {
                    meta.Add("format", elem.Attribute("meta.format").Value);
                }

                if (elem.Attribute("meta.type") != null)
                {
                    meta.Add("type", elem.Attribute("meta.type").Value);
                }

                if (elem.Attribute("meta.typeInfo") != null)
                {
                    meta.Add("typeInfo", elem.Attribute("meta.typeInfo").Value);
                }

                if (elem.Attribute("meta.orderId") != null)
                {
                    meta.Add("orderId", elem.Attribute("meta.orderId").Value);
                }

                if (elem.Attribute("meta.suppressCopy") != null)
                {
                    meta.Add("suppressCopy", elem.Attribute("meta.suppressCopy").Value);
                }

                if (elem.Attribute("meta.virtual") != null)
                {
                    meta.Add("virtual", elem.Attribute("meta.virtual").Value);
                }

                if (elem.Attribute("meta.virtualKey") != null)
                {
                    meta.Add("virtualKey", elem.Attribute("meta.virtualKey").Value);
                }

                if (elem.Attribute("meta.ignoreNull") != null)
                {
                    meta.Add("ignoreNull", elem.Attribute("meta.ignoreNull").Value);
                }

                if (elem.Attribute("meta.migMatch") != null)
                {
                    meta.Add("migMatch", elem.Attribute("meta.migMatch").Value);
                }

                if (elem.Attribute("meta.forceNameMatch") != null)
                {
                    meta.Add("forceNameMatch", elem.Attribute("meta.forceNameMatch").Value);
                }

                if (elem.Attribute("meta.ref") != null)
                {
                    meta.Add("ref", elem.Attribute("meta.ref").Value);
                }

                if (elem.Attribute("meta.objType") != null)
                {
                    meta.Add("objType", elem.Attribute("meta.objType").Value);
                }

                if (elem.Attribute("meta.sg") != null)
                {
                    meta.Add("sg", elem.Attribute("meta.sg").Value);
                }
                //check for parent sg
                else if (cur.Attribute("ref") != null)
                {
                    meta.Add("sg", cur.Attribute("ref").Value);
                }
                //add length, type? and list
                if (elem.Attribute("ref") != null)
                {
                    var refKey = elem.Attribute("ref").Value;
                    if (refKey.Contains("[")) //special case selection [blub=bla]
                    {
                        refKey = refKey.Split('[').FirstOrDefault();
                    }

                    if (refKey.Contains("(")) //special case multi-select (0,4)
                    {
                        var keyParts = refKey.Split('(');
                        refKey = keyParts[0] + keyParts[1].Split(',').FirstOrDefault();
                    }

                    var refKeys = refKey.Split(':');
                    meta.Add("segment", refKeys[0]);
                    var subElem = tree.FindElement(refKeys.First());
                    if (subElem != null)
                    {
                        if (subElem.APERAK_Check_String != null)
                        {
                            string[] elements;
                            if (subElem.APERAK_Check_String.Contains("|"))
                            {
                                elements = subElem.CONTRL_Check_String.Split('+');
                            }
                            else
                            {
                                elements = subElem.APERAK_Check_String.Split('+');
                            }

                            if (elements.Length >= int.Parse(refKeys[1]))
                            {
                                try
                                {
                                    var key = elements[int.Parse(refKeys[1]) - 1].Split('*')[
                                        int.Parse(refKeys[2]) + 1
                                    ];
                                    if (key.Contains("|"))
                                    {
                                        key = key.Split('|').First();
                                    }

                                    ParseAPERAKString(
                                        key,
                                        out var dataType,
                                        out var length,
                                        out var list
                                    );
                                    if (meta.Property("typeInfo") == null)
                                    {
                                        meta.Add("typeInfo", dataType);
                                    }

                                    meta.Add("length", length);
                                    if (list != null)
                                    {
                                        meta.Add("list", JArray.FromObject(list));
                                    }
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    // joscha said this is ok.
                                }
                                catch (Exception e)
                                {
                                    // errors must not pass silently
                                    Console.Out.WriteLineAsync($"Error while processing: {e}");
                                }
                            }
                        }
                    }
                    else if (refKey.StartsWith("SG"))
                    {
                        meta.Add("ref", refKey);
                    }
                }

                if (meta.Properties().Any())
                {
                    newField.Add("_meta", meta);
                }

                if (elem.Attribute("ahbName") != null)
                {
                    newField.Add("_ahbName", elem.Attribute("ahbName").Value);
                }

                if (elem.Attribute("groupBy") != null)
                {
                    var groupName = elem.Attribute("groupBy").Value;
                    var subGroup = refObj
                        .Children()
                        .Where(child =>
                            (child as JObject)?.Property("key")?.Value.Value<string>() == groupName
                        );
                    if (subGroup.Any())
                    {
                        if (elem.Attribute("groupKey")?.Value != null)
                        {
                            (subGroup.First() as JObject)
                                .Property("_meta")
                                .Value.Value<JObject>()
                                .Add("key", name);
                        }
                        (subGroup.First() as JObject)
                            .Property("requires")
                            .Value.Value<JArray>()
                            .Add(newField);
                    }
                    else
                    {
                        var subObj = new JObject { { "key", groupName } };
                        var subArray = new JArray();
                        var subMeta = new JObject { { "type", "group" }, { "max", "1" } };
                        if (elem.Attribute("groupKey")?.Value != null)
                        {
                            subMeta.Add("key", name);
                        }

                        if (elem.Attribute("groupCounterKey")?.Value != null)
                        {
                            subMeta.Add("counterKey", name);
                        }

                        subObj.Add("_meta", subMeta);
                        subObj.Add("requires", subArray);
                        subArray.Add(newField);
                        refObj.Add(subObj);
                    }
                }
                else
                {
                    refObj.Add(newField);
                }

                i--;
            }

            var j = cur.Descendants("class").Count(d => d.Parent == cur);
            if (j == 0 && !cur.Descendants("field").Any(d => d.Parent == cur)) // bei keinen fields und classes einen Key einfügen (momentan nur für Kontakt notwendig)
            { }

            foreach (var elem in cur.Descendants("class").Where(d => d.Parent == cur))
            {
                //class Elemente mappen
                j--;
                var newChild = new JObject();
                var name = elem.Attribute("name").Value;
                if (elem.Attribute("ahbName") != null)
                {
                    name = elem.Attribute("ahbName").Value;
                }

                newChild.Add("key", name);
                var meta = new JObject();
                if (elem.Attribute("meta.id") != null)
                {
                    meta.Add("id", elem.Attribute("meta.id").Value);
                }

                if (elem.Attribute("meta.help") != null)
                {
                    meta.Add("help", elem.Attribute("meta.help").Value);
                }

                if (elem.Attribute("meta.format") != null)
                {
                    meta.Add("format", elem.Attribute("meta.format").Value);
                }

                meta.Add(
                    "type",
                    elem.Attribute("meta.type") != null
                        ? elem.Attribute("meta.type").Value
                        : "group"
                );
                if (elem.Attribute("meta.objType") != null)
                {
                    meta.Add("objType", elem.Attribute("meta.objType").Value);
                }

                if (elem.Attribute("meta.typeInfo") != null)
                {
                    meta.Add("typeInfo", elem.Attribute("meta.typeInfo").Value);
                }

                if (elem.Attribute("meta.suppressCopy") != null)
                {
                    meta.Add("suppressCopy", elem.Attribute("meta.suppressCopy").Value);
                }

                if (elem.Attribute("meta.virtual") != null)
                {
                    meta.Add("virtual", elem.Attribute("meta.virtual").Value);
                }

                if (elem.Attribute("meta.virtualKey") != null)
                {
                    meta.Add("virtualKey", elem.Attribute("meta.virtualKey").Value);
                }

                if (elem.Attribute("meta.ref") != null)
                {
                    meta.Add("ref", elem.Attribute("meta.ref").Value);
                }

                if (elem.Attribute("meta.ignore") != null)
                {
                    meta.Add("ignore", elem.Attribute("meta.ignore").Value);
                }

                if (elem.Attribute("meta.changeTrigger") != null)
                {
                    meta.Add("changeTrigger", elem.Attribute("meta.changeTrigger").Value);
                }

                meta.Add("max", elem.Attribute("max") != null ? elem.Attribute("max").Value : "1");
                //find key element in fields
                var key = elem.Attribute("key")?.Value;
                if (key != null)
                {
                    var keyParts = key.Split('[');
                    var keyField = elem.Descendants("field")
                        .FirstOrDefault(field => field.Attribute("ref")?.Value == keyParts.First());
                    if (keyField != null)
                    {
                        if (keyField.Attribute("ahbName") != null)
                        {
                            meta.Add("groupKey", keyField.Attribute("ahbName")?.Value);
                        }
                        else
                        {
                            meta.Add("groupKey", keyField.Attribute("name")?.Value);
                        }
                    }
                    else if (elem.Attribute("groupKey") != null)
                    {
                        meta.Add("groupKey", elem.Attribute("groupKey")?.Value);
                    }
                }
                else if (elem.Attribute("groupKey") != null)
                {
                    meta.Add("groupKey", elem.Attribute("groupKey")?.Value);
                }

                if (elem.Attribute("groupCounterKey") != null)
                {
                    meta.Add("counterKey", elem.Attribute("groupCounterKey")?.Value);
                }

                if (elem.Attribute("meta.sg") != null)
                {
                    meta.Add("sg", elem.Attribute("meta.sg").Value);
                }
                else if (elem.Attribute("ref") != null)
                {
                    meta.Add("sg", elem.Attribute("ref")?.Value);
                }

                if (meta.Properties().Any())
                {
                    newChild.Add("_meta", meta);
                }

                var refKey = elem.Attribute("ref").Value;
                var refKeys = refKey.Split(':');
                var newTree = tree.FindElement(refKeys.First());
                if (newTree != null)
                {
                    tree = newTree;
                }
                else //recurse up the tree again
                {
                    var treeUp = tree;
                    while (newTree == null && treeUp.Parent != null)
                    {
                        newTree = treeUp.Parent.FindElement(refKeys.First());
                        if (newTree == null)
                        {
                            treeUp = treeUp.Parent;
                        }
                    }

                    if (newTree != null)
                    {
                        tree = newTree;
                    }
                }

                var requires = new JArray();

                //"requires": [ { name: [] } ]
                newChild.Add("requires", requires);
                var self = new JObject();
                requires.Add(self);
                var subs = new JArray();
                self.Add(elem.Attribute("name").Value, subs);
                Recurse(elem, subs, tree);

                refObj.Add(newChild);
            }
        }

        // Todo @JoschaMetze: add docstrings
        public string ConvertFilesToJSON(
            string inputFileName,
            string outputFileName,
            string treeFileName
        )
        {
            var srcXml = XElement.Parse(File.ReadAllText(inputFileName));
            TreeElement tree = null;
            if (!string.IsNullOrEmpty(treeFileName))
            {
                tree = new GenericEDILoader().LoadTree(File.ReadAllText(treeFileName));
            }
            var tmp = new JArray();
            Recurse(srcXml, tmp, tree);
            string version = RetrieveFormatVersionFromInputFileName(inputFileName);
            ((tmp[0] as JObject)["_meta"] as JObject).Add("version", version);
            File.WriteAllText(outputFileName, JsonConvert.SerializeObject(tmp));
            return JsonConvert.SerializeObject(tmp);
        }

        /// <summary>
        /// Extract the Format version (e.g. "1.0a") from a given template file name (e.g. "COMDIS1.0a.template")
        /// </summary>
        /// <param name="inputFileName">path of name of the template file</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string RetrieveFormatVersionFromInputFileName(string inputFileName)
        {
            var actualFileName = inputFileName
                .Split(new[] { ".template" }, StringSplitOptions.None)[0]
                .Split(Path.DirectorySeparatorChar)
                .Last();
            var match = FormatVersionRegex.Match(actualFileName);
            if (match.Success)
            {
                return match.Groups["version"].Value;
            }

            if (inputFileName.Contains(".create.template"))
            {
                throw new ArgumentException(
                    "Pass the name of the \".template\" file, not \".create.template\"",
                    nameof(inputFileName)
                );
            }
            throw new ArgumentException(
                $"Format version could not be determined: {actualFileName}",
                nameof(inputFileName)
            );
        }

        // the format version will be written to the json template file, thus needs to be passed
        public string ConvertToJSON(string xmlTemplate, string treeTemplate, string formatVersion)
        {
            var srcXml = XElement.Parse(xmlTemplate);
            TreeElement tree = null;
            tree = new GenericEDILoader().LoadTree(treeTemplate);
            var tmp = new JArray();
            Recurse(srcXml, tmp, tree);
            //retriev version information from inputFileName
            ((tmp[0] as JObject)["_meta"] as JObject).Add("version", formatVersion);
            return JsonConvert.SerializeObject(tmp);
        }
    }
}
