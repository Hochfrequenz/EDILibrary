// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace EDILibrary
{
    [DataContract]
    public class IEdiObject
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string MigName { get; set; }
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Edi { get; set; }
        public IEdiObject Parent { get; private set; }
        XElement template;
        [DataMember]
        public List<IEdiObject> Children { get; set; }
        public List<IEdiObject> SelfOrChildren
        {
            get { List<IEdiObject> list = Children.Take(Children.Count).ToList(); list.Add(this); return list; }
        }
        [DataMember]
        public Dictionary<string, List<string>> Fields { get; set; }
        [DataMember]
        public Dictionary<string, string> MigFields { get; set; }
        [DataMember]
        public Dictionary<string, string> EdiFields { get; set; }
        public IEdiObject(EDIEnums enumValue, XElement temp, string keyValue) : this(EDIEnumHelper.GetDescription(enumValue), temp, keyValue)
        {

        }
        public IEdiObject(string name, XElement temp, string keyValue)
        {
            Name = name;
            Children = new List<IEdiObject>();
            Fields = new Dictionary<string, List<string>>();
            MigFields = new Dictionary<string, string>();
            EdiFields = new Dictionary<string, string>();
            template = temp;
            Key = keyValue;

        }
        protected static string GenerateKey(string objectName)
        {
            switch (objectName)
            {
                case "Dokument":
                case "Nachricht":
                case "Vorgang":
                    {
                        return Guid.NewGuid().ToString("N").Substring(0, 13).ToUpper();
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        protected void ParseJSON(dynamic json)
        {

            foreach (var child in json.Children<JProperty>())
            {
                if (child.Value.GetType() == typeof(JArray))
                {
                    foreach (var childValue in child.Value)
                    {
                        IEdiObject childNode = new IEdiObject(child.Name, null, GenerateKey(child.Name));
                        childNode.ParseJSON(childValue);
                        if (childNode.Fields.Count > 0 || childNode.Children.Count > 0)
                            AddChild(childNode);
                    }
                }
                else
                {
                    if (!(child.Value == null || child.Type == JTokenType.Null))
                        Fields.Add(child.Name, new List<string> { child.Value.ToString() });
                }
            }

        }
        public static IEdiObject CreateFromJSON(string JSON)
        {
            dynamic json = JsonConvert.DeserializeObject(JSON);
            var doc = json["Dokument"][0];
            IEdiObject root = new IEdiObject(EDIEnums.Dokument, null, GenerateKey("Dokument"));
            root.ParseJSON(doc);
            return root;
        }
        public bool IsEqual(IEdiObject comp)
        {
            if (Name != comp.Name)
                return false;
            if (Fields.Count != comp.Fields.Count)
                return false;
            if (Children.Count != comp.Children.Count)
                return false;
            foreach (var child in Children)
            {
                var compChild = comp.Children.FirstOrDefault(ch => ch.Name == child.Name && ch.Key == child.Key);
                if (compChild == null)
                    return false;
                if (!child.IsEqual(compChild))
                    return false;
            }
            foreach (var field in Fields)
            {
                var compField = comp.Fields[field.Key];
                if (compField == null)
                    return false;
                else
                {
                    if (field.Key != "Dokumentendatum")
                        if (compField.First() != field.Value.First())
                            return false;
                }
            }
            return true;
        }
        public override string ToString()
        {
            return Name + "  " + Key;
        }
        protected void Recurse(XElement elem, IEdiObject child)
        {
            foreach (var field in child.Fields)
            {
                XElement f = new XElement("Field");
                f.SetAttributeValue("name", field.Key);
                f.Value = string.Join("|", field.Value);
                elem.Add(f);
            }
            foreach (var cl in child.Children)
            {
                XElement ch = new XElement("Class");
                ch.SetAttributeValue("name", cl.Name);
                ch.SetAttributeValue("key", cl.Key);
                Recurse(ch, cl);
                elem.Add(ch);
            }
        }
        protected StringBuilder _builder;
        protected void RecurseJSON(IEdiObject cur)
        {
            bool hasKey = false;

            int i = cur.Fields.Count;
            bool hasClass = cur.Children.Count > 0;
            if (cur.Key != null)
            {
                hasKey = true;
                _builder.AppendLine("\"" + "Key" + "\" : \"" + cur.Key + "\"" + (i != 0 || hasClass ? "," : ""));
            }
            if (cur.Fields.Count(f => f.Value.Count > 1) == cur.Fields.Count && cur.Fields.Any(f => f.Value.Count > 1)) // check for multiple values
            {
                int index = 0;
                var oldI = i;
                while (index < cur.Fields.First(f => f.Value.Count > 1).Value.Count)
                {
                    foreach (var elem in cur.Fields)
                    {
                        i--;
                        _builder.AppendLine("\"" + elem.Key + "\" : \"" + elem.Value.ElementAt(index) + "\"" + (i != 0 || hasClass ? "," : ""));

                    }
                    i = oldI;
                    if (index + 1 < cur.Fields.First(f => f.Value.Count > 1).Value.Count)
                        _builder.AppendLine("},{");
                    index++;
                }
            }
            else
            {
                foreach (var elem in cur.Fields)
                {
                    i--;
                    _builder.AppendLine("\"" + elem.Key + "\" : \"" + elem.Value.FirstOrDefault() + "\"" + (i != 0 || hasClass ? "," : ""));

                }
            }
            int j = cur.Children.Count;
            if (j == 0 && cur.Fields.Count == 0) // bei keinen fields und classes einen Key einfügen (momentan nur für Kontakt notwendig)
            {
                if (!hasKey)
                {
                    string key = "";
                    if (cur.Key != null)
                    {
                        key = "\"" + "Key" + "\" : \"" + cur.Key + "\"" + (i != 0 || hasClass ? "," : "");
                    }
                    else
                    {
                        key = "\"" + "Key" + "\" : \"" + "" + "\"" + (i != 0 || hasClass ? "," : "");
                    }
                    _builder.AppendLine(key);
                }

            }
            var lastName = "";
            bool openElement = false;
            foreach (var elem in cur.Children)
            {
                if (elem.Name != lastName && lastName != "")
                {
                    _builder.AppendLine("]" + (j != 0 ? "," : ""));
                    openElement = false;
                }
                else if (lastName != "")
                    _builder.AppendLine(",{");
                j--;
                if (elem.Name != lastName)
                {
                    _builder.AppendLine("\"" + elem.Name + "\" :  [{");
                    openElement = true;
                }
                RecurseJSON(elem);
                _builder.AppendLine("}");
                lastName = elem.Name;
            }
            if (openElement)
                _builder.AppendLine("]" + (j != 0 ? "," : ""));
        }
        protected void RecurseJSON(XElement cur)
        {
            bool hasKey = false;

            int i = cur.Descendants("Field").Count(d => d.Parent == cur);
            bool hasClass = cur.Descendants("Class").Any(d => d.Parent == cur);
            if (cur.Attribute("key") != null)
            {
                hasKey = true;
                _builder.AppendLine("\"" + "Key" + "\" : \"" + cur.Attribute("key").Value + "\"" + (i != 0 || hasClass ? "," : ""));
            }
            foreach (var elem in cur.Descendants("Field").Where(d => d.Parent == cur))
            {
                i--;
                _builder.AppendLine("\"" + elem.Attribute("name").Value + "\" : \"" + elem.Value + "\"" + (i != 0 || hasClass ? "," : ""));

            }
            int j = cur.Descendants("Class").Count(d => d.Parent == cur);
            if (j == 0 && cur.Descendants("Field").All(d => d.Parent != cur)) // bei keinen fields und classes einen Key einfügen (momentan nur für Kontakt notwendig)
            {
                if (!hasKey)
                {
                    string key = "";
                    if (cur.Attribute("key") != null)
                    {
                        key = "\"" + "Key" + "\" : \"" + cur.Attribute("key").Value + "\"" + (i != 0 || hasClass ? "," : "");
                    }
                    else
                    {
                        key = "\"" + "Key" + "\" : \"" + "" + "\"" + (i != 0 || hasClass ? "," : "");
                    }
                    _builder.AppendLine(key);
                }

            }
            foreach (var elem in cur.Descendants("Class").Where(d => d.Parent == cur))
            {
                j--;
                _builder.AppendLine("\"" + elem.Attribute("name").Value + "\" :  [{");
                RecurseJSON(elem);
                _builder.AppendLine("}]" + (j != 0 ? "," : ""));
            }
        }
        public string Serialize()
        {
            XElement root = new XElement("EDIFACT");
            Recurse(root, this);
            return root.ToString();
        }

        public string SerializeToJSON()
        {
            _builder = new StringBuilder();
            _builder.AppendLine("{");
            _builder.AppendLine("\"Dokument\":[{");
            RecurseJSON(this);
            _builder.AppendLine("}]}");
            return _builder.ToString();
        }
        public bool ContainsField(string name)
        {
            return Fields.ContainsKey(name);
        }
        public bool ContainsField(EDIEnums enumValue)
        {
            return Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue));
        }
        public string[] FieldList(EDIEnums enumValue)
        {
            if (Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue)))
            {
                return Fields[EDIEnumHelper.GetDescription(enumValue)].ToArray<string>();
            }
            else
                return null;
        }
        public string[] FieldList(string name)
        {
            if (Fields.ContainsKey(name))
            {
                return Fields[name].ToArray<string>();
            }
            else
                return null;
        }
        public string Field(EDIEnums enumValue)
        {
            if (Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue)))
            {
                return Fields[EDIEnumHelper.GetDescription(enumValue)].First();
            }
            else
                return null;
        }
        public string Field(string name)
        {
            if (Fields.ContainsKey(name))
            {
                return Fields[name].First();
            }
            else
                return null;
        }

        public string Field(EDIEnums enumValue, int index)
        {
            if (Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue)))
            {
                return Fields[EDIEnumHelper.GetDescription(enumValue)][index];
            }
            else
                return null;
        }
        public void AddField(EDIEnums enumValue, string value)
        {
            if (Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue)))
            {
                Fields[EDIEnumHelper.GetDescription(enumValue)] = new List<string> { value };
            }
            else
            {
                Fields.Add(EDIEnumHelper.GetDescription(enumValue), new List<string> { value });
            }
        }
        public IEdiObject Child(EDIEnums name, string key)
        {
            return Child(EDIEnumHelper.GetDescription(name), key);
        }
        public IEdiObject Child(EDIEnums name)
        {
            return Child(EDIEnumHelper.GetDescription(name));
        }
        public IEdiObject Child(string name, string key)
        {
            return (from child in Children where child.Name == name && child.Key == key select child).FirstOrDefault();
        }
        public IEdiObject Child(string name)
        {
            return (from child in Children where child.Name == name select child).FirstOrDefault();
        }
        public void AddChild(IEdiObject child)
        {
            if (child == null)
                return;
            child.Parent = this;
            Children.Add(child);
        }
        public bool ContainsChild(EDIEnums name, string key)
        {
            return ContainsChild(EDIEnumHelper.GetDescription(name), key);
        }
        public bool ContainsChild(EDIEnums name)
        {
            return ContainsChild(EDIEnumHelper.GetDescription(name));
        }
        public bool ContainsChild(string name, string key)
        {
            return (from child in Children where child.Name == name && child.Key == key select child).Count() > 0;
        }
        public bool ContainsChild(string name)
        {
            return (from child in Children where child.Name == name select child).Count() > 0;
        }
        public void RemoveField(string name)
        {
            Fields.Remove(name);
        }
        public void RemoveChilds(EDIEnums name)
        {
            RemoveChilds(EDIEnumHelper.GetDescription(name));
        }
        public void RemoveChilds(string name)
        {
            var children = (from child in Children where child.Name == name select child).ToList();
            foreach (IEdiObject child in children)
            {
                Children.Remove(child);
            }
        }
        public List<IEdiObject> Childs(EDIEnums name)
        {
            return Childs(EDIEnumHelper.GetDescription(name));
        }
        public List<IEdiObject> Childs(string name)
        {
            return (from child in Children where child.Name == name select child).ToList();
        }
        public IEdiObject Clone()
        {
            IEdiObject clone = new IEdiObject(Name, null, Key);
            foreach (var child in Children)
            {
                clone.AddChild(child.Clone());
            }
            foreach (var field in Fields)
            {
                clone.Fields.Add(field.Key, new List<string>(field.Value));
            }
            return clone;
        }
    }
}
