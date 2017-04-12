using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace EDILibrary
{
    public class APERAKDescriptionAttribute : Attribute
    {
        public APERAKDescriptionAttribute(string desc)
        {
            Description = desc;
        }
        public string Description { get; set; }
    }
    public class EDIDescriptionAttribute :Attribute
    {
        public EDIDescriptionAttribute(string desc)
        {
            Description = desc;
        }
        public string Description { get; set; }
    }
    public class EDIEnumHelper
    {
        public static Dictionary<string, string> DescriptionMap = null;
        public static string GetDescription(string name)
        {
            if (DescriptionMap == null)
            {
                DescriptionMap = new Dictionary<string, string>();
                FieldInfo[] fields = typeof(EDIEnums).GetRuntimeFields().ToArray();
                foreach (FieldInfo field in fields)
                {
                    object att = field.GetCustomAttributes(typeof(DescriptionAttribute),false).FirstOrDefault();
                    if (att != null)
                    {
                        DescriptionMap[(att as DescriptionAttribute).Description] = field.Name;
                    }
                }
            }
            IEnumerable<Attribute> attrs =
                typeof(EDIEnums).GetRuntimeField(DescriptionMap[name]).GetCustomAttributes(typeof(EDIDescriptionAttribute), false);
            if (attrs != null)
            {
                return (attrs.Count() > 0) ? ((EDIDescriptionAttribute)attrs.First()).Description : name;
            }
            return name;

        }
        public static string GetAPERAKDescription(EDIEnums enumValue)
        {
            string name = enumValue.ToString();
            IEnumerable<Attribute> attrs = 
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(APERAKDescriptionAttribute), false);
            if (attrs != null)
            {
                return (attrs.Count() > 0) ? ((APERAKDescriptionAttribute)attrs.First()).Description : name;
            }
            else
            {
                attrs =
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attrs.Count() > 0) ? "FEHLER:"+((DescriptionAttribute)attrs.First()).Description : "FEHLER:"+name;
            }
        }
        public static string GetEDIDescription(EDIEnums enumValue)
        {
            string name = enumValue.ToString();
            IEnumerable<Attribute> attrs = 
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(EDIDescriptionAttribute), false);
            if (attrs != null)
            {
                return (attrs.Count() > 0) ? ((EDIDescriptionAttribute)attrs.First()).Description : name;
            }
            else
            {
                attrs =
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attrs.Count() > 0) ? ((DescriptionAttribute)attrs.First()).Description : name;
            }
        }
        public static string GetDescription(EDIEnums enumValue)
        {
            string name = enumValue.ToString();
            IEnumerable<Attribute> attrs = 
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attrs.Count() > 0) ? ((DescriptionAttribute)attrs.First()).Description : name;
        }
    }
    public enum EDIEnums
    {
        Dokument
    }
}
