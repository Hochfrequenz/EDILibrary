using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
namespace EDILibrary.Repositories
{
    public class BaseKeyValue : IComparable
    {

        private string _key;
        private string _value;
        private string _sparte;
        private string _bis ="12/99";
        private string _ab ="00/00";
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        public string Value
        {
            get
            {
                if (Customization.Instance[CustomizingOptions.ExpertMode] as string == "true")
                    return _value + " - " + _key;
                else
                    return _value;
            }
            set
            {
                string[] valueSplit = value.Split(new string[] { "-" }, StringSplitOptions.None);
                if (valueSplit.Count() > 1)
                    _value = String.Join("-", valueSplit);
                else
                    _value = value;
            }
        }

        public string Bis
        {
            get
            {
                return _bis;
            }

            set
            {
                _bis = value;
            }
        }

        public string Ab
        {
            get
            {
                return _ab;
            }

            set
            {
                _ab = value;
            }
        }

        public string Sparte
        {
            get
            {
                return _sparte;
            }

            set
            {
                _sparte = value;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public bool IsValid(string sparte,string formatVersion)
        {
            bool valid = true;
            if(!String.IsNullOrEmpty(sparte))
            {
                if (Sparte != sparte)
                    valid = false;
            }
            if (!String.IsNullOrEmpty(formatVersion))
            {
                formatVersion.DashDelimitedBetween(Ab, Bis);
            }
            return valid;
        }
        public override bool Equals(object obj)
        {
            if (obj != null && (obj.GetType().GetTypeInfo().IsSubclassOf(typeof(BaseKeyValue)) || obj.GetType() == (typeof(BaseKeyValue))))
            {
                BaseKeyValue comp = (BaseKeyValue)obj;
                //Anpassung 20121204: Nur den Key vergleichen, da durch die Serialisierung und die
                //Expertenhilfe sonst falsche Vergleiche entstehen
                return (_key == comp._key);
            }
            else
                return base.Equals(obj);
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(string))
            {
                return _key.CompareTo(obj as string);
            }
            else if (obj.GetType() == typeof(BaseKeyValue) || obj.GetType().GetTypeInfo().IsSubclassOf(typeof(BaseKeyValue)))
            {
                return _key.CompareTo((obj as BaseKeyValue).Key);
            }
            return 0;
        }
    }
}
