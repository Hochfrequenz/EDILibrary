// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

namespace EDILibrary
{
    public class PathSelection
    {
        public EdiObject Child { get; set; }
        public EdiObject Parent { get; set; }
        public string Field { get; set; }
        public string FieldName { get; set; }
    }
}
