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
        
        public readonly string _zuLang = null;
        protected List<XElement> _mappingRoot = new List<XElement>();

        public ExtendedMappings()
        {
            _zuLang = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";
        }
        public List<string> GetListOfMappings()
        {
            return _mappingRoot.Select(mr => mr.Attribute("Name").Value).ToList<string>();
        }
        public List<MappingEntry> GetListOfMappingTypes()
        {
            return _mappingRoot.Select(mr => new MappingEntry()
            {
                Name = mr.Attribute("Name").Value,
                Type = mr.Attribute("type") != null ? mr.Attribute("type").Value : "python",
                Format = mr.Attribute("format")?.Value
            }).ToList<MappingEntry>();
        }
        public void LoadMappings(string mappingXML, int iClient)
        {
            _mappingRoot = XElement.Parse(mappingXML).Descendants("Mapping").Where(dsc => dsc.Attribute("Client").Value == iClient.ToString()).ToList();
        }
        public void LoadMappings(string mappingXML)
        {
            if (string.IsNullOrEmpty(mappingXML))
                return;
            _mappingRoot = XElement.Parse(mappingXML).Descendants("Mapping").ToList();

        }
        public string GetMapping(string mappingName)
        {
            XElement mapping = _mappingRoot.Where(mr => mr.Attribute("Name").Value == mappingName).FirstOrDefault();
            if (mapping != null)
            {
                return mapping.Value;
            }
            else
                return null;
        }
        public void OverrideMapping(string mappingName, string newMapping)
        {
            XElement mapping = _mappingRoot.Where(mr => mr.Attribute("Name").Value == mappingName).FirstOrDefault();
            if (mapping != null)
            {
                mapping.Value = newMapping;
            }
        }

        public void ExecuteMapping(string mappingName, IEdiObject obj, string sparte, string format)
        {
            XElement mapping = _mappingRoot.Where(mr => mr.Attribute("Name").Value == mappingName && (mr.Attribute("format") == null || mr.Attribute("format").Value == format)).FirstOrDefault();
            if (mapping != null)
            {
                if (mapping.Attribute("type") == null || mapping.Attribute("type").Value == "python")
                {
                    throw new NotImplementedException("Der .net Core-Port unterstützt kein IronPython");
                }
            }
        }
        protected List<string> _ediLines = null;
        protected string GetValue(string pos, string edi)
        {
            //if (_valueCache.ContainsKey(edi))
            //{
            //    if (_valueCache[edi].ContainsKey(pos))
            //    {
            //        return _valueCache[edi][pos];
            //    }
            //}
            edi = edi.Replace("??", "<<").Replace("?+", "?<").Replace("?:", "?>");
            if (pos == null || pos == "")
                return null;
            string[] Groups = edi.Split(new[] { '+' });
            string[] SubPos = pos.Split(new[] { ':' });
            if (!edi.StartsWith(SubPos[0]))
                return null;
            int GroupPos = int.Parse(SubPos[1]);
            if (Groups.Length <= GroupPos)
                return null;
            string[] SubGroups = Groups[GroupPos].Split(new[] { ':' });
            if (SubPos[2].Contains("("))
            {
                string[] range = SubPos[2].Split(new[] { ',' });
                int start = int.Parse(range[0].Substring(1));
                int end = int.Parse(range[1].Substring(0, range[1].Length - 1));
                List<string> parts = new List<string>();
                for (int i = start; i <= end; i++)
                {
                    if (SubGroups.Length <= i)
                        break;
                    parts.Add(SubGroups[i].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'").Replace("<<", "?"));
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
            else
            {
                int DetailPos = int.Parse(SubPos[2]);
                if (SubGroups.Length <= DetailPos)
                    return null;
                string result = SubGroups[DetailPos].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'").Replace("<<", "?");
                //if (!_valueCache.ContainsKey(edi))
                //{
                //    _valueCache.Add(edi, new Dictionary<string, string>());
                //}
                //_valueCache[edi][pos] = result.Trim();
                return Escape(result.Trim());
            }
        }
        public void PrepareEDIMapping(string edi)
        {
            _ediLines = new List<string>();
            _ediLines = edi.Split(new[] { "'" }, StringSplitOptions.None).ToList<string>();
        }
        public string Escape(string input)
        {
            return input.Replace("+", "?+").Replace(":", "?:").Replace("'", "?'");
        }
        public void ExecuteEDIMapping(string mappingName)
        {
            if (_ediLines == null)
                throw new Exception("Call PrepareEDIMapping before executing a mapping");
            XElement mapping = _mappingRoot.Where(mr => mr.Attribute("Name").Value == mappingName).FirstOrDefault();
            if (mapping != null)
            {
                if (mapping.Attribute("type") != null && mapping.Attribute("type").Value == "edi")
                {
                    var parts = mapping.Value.Replace("\n", "").Split(new[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
                    var selector = parts[0].Trim();
                    var newValue = "";
                    if (parts.Count() > 1)
                        newValue = parts[1].Trim();
                    if (newValue == "<zuLang>")
                    {
                        newValue = _zuLang;
                    }
                    int klammerIndex = selector.IndexOf('[');
                    if (klammerIndex == -1)
                        klammerIndex = selector.Length;
                    string selection = selector.Substring(0, klammerIndex);
                    string path = null;
                    int sepIndex = selection.IndexOf(':');
                    if (sepIndex == -1)
                        sepIndex = selection.Length;
                    string segment = selection.Substring(0, sepIndex);
                    if (klammerIndex != selector.Length)
                        path = selector.Substring(klammerIndex + 1, selector.Length - 1 - klammerIndex - 1);
                    // string[] paths = path.Split(new char[] { '^', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    string path_selector = null;
                    string path_value = null;
                    if (path != null)
                    {
                        string sep_op = "=";
                        if (path.Contains("!="))
                        {
                            sep_op = "!=";

                        }
                        int opIndex = path.IndexOf(sep_op);
                        if (opIndex == -1)
                            opIndex = path.Length;
                        path_selector = path.Substring(0, opIndex);

                        if (path.Length != sepIndex)
                            path_value = path.Substring(opIndex + sep_op.Length, path.Length - opIndex - sep_op.Length);
                        else
                        {
                            path_value = "";
                        }
                    }
                    for (int i = 0; i < _ediLines.Count; i++)
                    {

                        var edi_segment = _ediLines[i];
                        if (edi_segment.StartsWith(segment) == false)
                            continue;
                        if (path != null)
                        {
                            var selection_path = path_selector;
                            if (path_selector.Split(new[] { ':' }).Count() <= 2)
                            {
                                selection_path = segment + ":" + path_selector;
                            }
                            if (GetValue(selection_path, edi_segment) == path_value)
                            {
                                var value = GetValue(selection, edi_segment);
                                if (newValue != "<entfernen>")
                                    _ediLines[i] = edi_segment.Replace(value, newValue);
                                else
                                    _ediLines[i] = "";
                            }
                        }
                        else
                        {
                            var value = GetValue(selection, edi_segment);
                            if (newValue != "<entfernen>")
                                _ediLines[i] = edi_segment.Replace(value, newValue);
                            else
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
