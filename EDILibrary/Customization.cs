// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary
{
    public enum CustomizingOptions
    {
        ExpertMode
    }
    public class Customization
    {

        protected static Customization _instance = null;
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
            get
            {
                if (_instance == null)
                    _instance = new Customization();

                return _instance;
            }
        }
    }
}
