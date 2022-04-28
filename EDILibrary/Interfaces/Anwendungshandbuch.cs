using System.Collections.Generic;

namespace EDILibrary.MAUS
{

    public class Anwendungshandbuch
    {
        public List<SegmentGroup> Lines { get; set; }
        public Meta Meta { get; set; }
    }

    //public class Line
    //{
    //    [System.Text.Json.Serialization.JsonPropertyName("ahb_expression")]
    //    public string AhbExpression { get; set; }
    //    public string Discriminator { get; set; }
    //    [System.Text.Json.Serialization.JsonPropertyName("segment_groups")]
    //    public List<SegmentGroup> SegmentGroups { get; set; }
    //    public List<Segment> Segments { get; set; }
    //}

    public class SegmentGroup
    {
        [System.Text.Json.Serialization.JsonPropertyName("ahb_expression")]
        public string AhbExpression { get; set; }
        public string Discriminator { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("segment_groups")]
        public List<SegmentGroup> SegmentGroups { get; set; }
        public List<Segment> Segments { get; set; }
    }

    public class Segment
    {
        public string AhbExpression { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("data_elements")]
        public List<DataElement> DataElements { get; set; }
        public string Discriminator { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("section_name")]
        public string SectionName { get; set; }
    }

    public class DataElement
    {
        [System.Text.Json.Serialization.JsonPropertyName("data_element_id")]
        public string DataElementId { get; set; }
        public string Discriminator { get; set; }

    }

    public class FreeText : DataElement
    {
        [System.Text.Json.Serialization.JsonPropertyName("ahb_expression")]
        public string AhbExpression { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("entered_input")]
        public string EnteredInput { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("value_type")]
        public ValueType? ValueType { get; set; }
    }

    public class DataElementValuePool : DataElement
    {
        [System.Text.Json.Serialization.JsonPropertyName("value_pool")]
        public List<ValuePoolElement> ValuePool { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("value_type")]
        public ValueType? ValueType { get; set; }
    }

    public class ValuePoolElement
    {
        [System.Text.Json.Serialization.JsonPropertyName("ahb_expression")]
        public string AhbExpression { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("edifact_key")]
        public string EdifactKey { get; set; }
        public string Meaning { get; set; }
    }

    public class Meta
    {
        public string Pruefidentifikator { get; set; }
    }

    public enum ValueType { Datetime, Text, ValuePool };
}
