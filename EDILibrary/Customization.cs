// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using System.Collections.Generic;

namespace EDILibrary
{
    public enum CustomizingOptions
    {
        ExpertMode
    }
    public class Customization
    {

        protected static Customization _instance;
        protected Dictionary<CustomizingOptions, object> _parameters = new Dictionary<CustomizingOptions, object>();
        public object this[CustomizingOptions index]
        {
            get
            {
                if (_parameters.ContainsKey(index))
                    return _parameters[index];
                else
                    return null;
            }
            set { _parameters[index] = value; }
        }
        public static Customization Instance
        {
            get { return _instance ?? (_instance = new Customization()); }
        }
    }
}
