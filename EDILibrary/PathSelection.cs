// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

namespace EDILibrary
{
    public class PathSelection
    {
        public IEdiObject Child { get; set; }
        public IEdiObject Parent { get; set; }
        public string Field { get; set; }
        public string FieldName { get; set; }
    }
}
