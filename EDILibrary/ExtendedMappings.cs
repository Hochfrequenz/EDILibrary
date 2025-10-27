// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace EDILibrary
{
    public class MappingEntry
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
    }

    public class ExtendedMappings
    {
        public readonly string _zuLang;
        protected List<XElement> _mappingRoot = new List<XElement>();

        public ExtendedMappings()
        {
            _zuLang =
                "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";
        }

        public List<string> GetListOfMappings()
        {
            return _mappingRoot.Select(mr => mr.Attribute("Name").Value).ToList();
        }

        public List<MappingEntry> GetListOfMappingTypes()
        {
            return _mappingRoot
                .Select(mr => new MappingEntry
                {
                    Name = mr.Attribute("Name").Value,
                    Type = mr.Attribute("type") != null ? mr.Attribute("type").Value : "python",
                    Format = mr.Attribute("format")?.Value,
                })
                .ToList();
        }

        public void LoadMappings(string mappingXML, int iClient)
        {
            _mappingRoot = XElement
                .Parse(mappingXML)
                .Descendants("Mapping")
                .Where(dsc => dsc.Attribute("Client").Value == iClient.ToString())
                .ToList();
        }

        public void LoadMappings(string mappingXML)
        {
            if (string.IsNullOrEmpty(mappingXML))
            {
                return;
            }

            _mappingRoot = XElement.Parse(mappingXML).Descendants("Mapping").ToList();
        }

        public string GetMapping(string mappingName)
        {
            var mapping = _mappingRoot.FirstOrDefault(mr =>
                mr.Attribute("Name").Value == mappingName
            );
            return mapping != null ? mapping.Value : null;
        }

        public void OverrideMapping(string mappingName, string newMapping)
        {
            var mapping = _mappingRoot.FirstOrDefault(mr =>
                mr.Attribute("Name").Value == mappingName
            );
            if (mapping != null)
            {
                mapping.Value = newMapping;
            }
        }

        public void ExecuteMapping(
            string mappingName,
            EdiObject obj,
            string sparte,
            EdifactFormat? format
        )
        {
            var mapping = _mappingRoot.FirstOrDefault(mr =>
                mr.Attribute("Name").Value == mappingName
                && (
                    mr.Attribute("format") == null
                    || Enum.Parse<EdifactFormat>(mr.Attribute("format").Value) == format
                )
            );
            if (mapping != null)
            {
                if (
                    mapping.Attribute("type") == null
                    || mapping.Attribute("type")?.Value == "python"
                )
                {
                    throw new NotImplementedException(
                        "Der .net Core-Port unterstützt kein IronPython"
                    );
                }
            }
        }

        protected List<string> _ediLines;

        protected static string GetValue(string pos, string edi)
        {
            //if (_valueCache.ContainsKey(edi))
            //{
            //    if (_valueCache[edi].ContainsKey(pos))
            //    {
            //        return _valueCache[edi][pos];
            //    }
            //}
            edi = edi.Replace("??", "<<").Replace("?+", "?<").Replace("?:", "?>");
            if (string.IsNullOrEmpty(pos))
            {
                return null;
            }

            string[] groups = edi.Split('+');
            string[] subPos = pos.Split(':');
            if (!edi.StartsWith(subPos[0]))
            {
                return null;
            }

            int groupPos = int.Parse(subPos[1]);
            if (groups.Length <= groupPos)
            {
                return null;
            }

            string[] subGroups = groups[groupPos].Split(':');
            if (subPos[2].Contains("("))
            {
                string[] range = subPos[2].Split(',');
                int start = int.Parse(range[0].Substring(1));
                int end = int.Parse(range[1].Substring(0, range[1].Length - 1));
                var parts = new List<string>();
                for (int i = start; i <= end; i++)
                {
                    if (subGroups.Length <= i)
                    {
                        break;
                    }

                    parts.Add(
                        subGroups[i]
                            .Replace("?<", "+")
                            .Replace("?>", ":")
                            .Replace("?$", "'")
                            .Replace("<<", "?")
                    );
                }
                //Abweichend zur Behandlung im EDIReader bleiben hier die :-Trennzeichen erhalten um eine korrekte Ersetzung sicherzustellen
                string endValue = string.Join(":", parts).Trim();
                //if (!_valueCache.ContainsKey(edi))
                //{
                //    _valueCache.Add(edi, new Dictionary<string, string>());
                //}
                //_valueCache[edi][pos] = endValue;
                return endValue;
            }

            int detailPos = int.Parse(subPos[2]);
            if (subGroups.Length <= detailPos)
            {
                return null;
            }

            string result = subGroups[detailPos]
                .Replace("?<", "+")
                .Replace("?>", ":")
                .Replace("?$", "'")
                .Replace("<<", "?");
            //if (!_valueCache.ContainsKey(edi))
            //{
            //    _valueCache.Add(edi, new Dictionary<string, string>());
            //}
            //_valueCache[edi][pos] = result.Trim();
            return Escape(result.Trim());
        }

        public void PrepareEDIMapping(string edi)
        {
            _ediLines = new List<string>();
            _ediLines = edi.Split(new[] { "'" }, StringSplitOptions.None).ToList();
        }

        public static string Escape(string input)
        {
            return input.Replace("+", "?+").Replace(":", "?:").Replace("'", "?'");
        }

        public void ExecuteEDIMapping(string mappingName)
        {
            if (_ediLines == null)
            {
                throw new Exception("Call PrepareEDIMapping before executing a mapping");
            }

            var mapping = _mappingRoot.FirstOrDefault(mr =>
                mr.Attribute("Name").Value == mappingName
            );
            if (mapping?.Attribute("type") != null && mapping.Attribute("type").Value == "edi")
            {
                string[] parts = mapping
                    .Value.Replace("\n", "")
                    .Split(new[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                string selector = parts[0].Trim();
                string newValue = "";
                if (parts.Length > 1)
                {
                    newValue = parts[1].Trim();
                }

                if (newValue == "<zuLang>")
                {
                    newValue = _zuLang;
                }
                int klammerIndex = selector.IndexOf('[');
                if (klammerIndex == -1)
                {
                    klammerIndex = selector.Length;
                }

                string selection = selector.Substring(0, klammerIndex);
                string path = null;
                int sepIndex = selection.IndexOf(':');
                if (sepIndex == -1)
                {
                    sepIndex = selection.Length;
                }

                string segment = selection.Substring(0, sepIndex);
                if (klammerIndex != selector.Length)
                {
                    path = selector.Substring(
                        klammerIndex + 1,
                        selector.Length - 1 - klammerIndex - 1
                    );
                }

                // string[] paths = path.Split(new char[] { '^', '|' }, StringSplitOptions.RemoveEmptyEntries);
                string pathSelector = null;
                string pathValue = null;
                if (path != null)
                {
                    string sepOp = "=";
                    if (path.Contains("!="))
                    {
                        sepOp = "!=";
                    }
                    int opIndex = path.IndexOf(sepOp);
                    if (opIndex == -1)
                    {
                        opIndex = path.Length;
                    }

                    pathSelector = path.Substring(0, opIndex);

                    if (path.Length != sepIndex)
                    {
                        pathValue = path.Substring(
                            opIndex + sepOp.Length,
                            path.Length - opIndex - sepOp.Length
                        );
                    }
                    else
                    {
                        pathValue = "";
                    }
                }
                for (int i = 0; i < _ediLines.Count; i++)
                {
                    string ediSegment = _ediLines[i];
                    if (ediSegment.StartsWith(segment) == false)
                    {
                        continue;
                    }

                    if (path != null)
                    {
                        string selectionPath = pathSelector;
                        if (pathSelector.Split(':').Length <= 2)
                        {
                            selectionPath = segment + ":" + pathSelector;
                        }
                        if (GetValue(selectionPath, ediSegment) == pathValue)
                        {
                            string value = GetValue(selection, ediSegment);
                            if (newValue != "<entfernen>")
                            {
                                _ediLines[i] = ediSegment.Replace(value, newValue);
                            }
                            else
                            {
                                _ediLines[i] = "";
                            }
                        }
                    }
                    else
                    {
                        string value = GetValue(selection, ediSegment);
                        if (newValue != "<entfernen>")
                        {
                            _ediLines[i] = ediSegment.Replace(value, newValue);
                        }
                        else
                        {
                            _ediLines[i] = "";
                        }
                    }
                }
            }
        }

        public string GetFinalEDIMapping()
        {
            return string.Join("'", _ediLines);
        }
    }
}
