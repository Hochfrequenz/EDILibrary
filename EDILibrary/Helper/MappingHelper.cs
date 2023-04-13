// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;

namespace EDILibrary.Helper
{
    public class MappingHelper
    {
        public static string ExecuteMappingsWithTemplates(EdiObject edi, EDIFileInfo fileInfo, List<string> mappings, string createTemplate, TimeZoneInfo localZone, bool bUseLocalTime = true)
        {
            var extMapping = new ExtendedMappings();
            extMapping.LoadMappings("");
            foreach (var map in mappings)
            {
                try
                {
                    extMapping.ExecuteMapping(map, edi, "S", fileInfo.Format);
                }
                catch (Exception) // todo: fix pokemon catching
                {
                }
            }
            var writer = new GenericEDIWriter();
            GenericEDIWriter.helper.useLocalTime = bUseLocalTime;
            GenericEDIWriter.helper.LocalTimeZone = localZone ?? TimeZoneInfo.Local;
            var newEDI = writer.CompileTemplate(createTemplate, edi);
            extMapping.PrepareEDIMapping(newEDI);
            foreach (var map in mappings)
            {
                try
                {
                    extMapping.ExecuteEDIMapping(map);
                }
                catch (Exception) // todo: fix pokemon catching
                {
                }
            }
            return extMapping.GetFinalEDIMapping();
        }
        public static string ExecuteMappings(EdiObject edi, EDIFileInfo fileInfo, List<string> mappings, string createTemplate, TimeZoneInfo localZone, bool bUseLocalTime = true)
        {
            var extMapping = new ExtendedMappings();
            extMapping.LoadMappings("");
            foreach (var map in mappings)
            {
                try
                {
                    extMapping.ExecuteMapping(map, edi, "S", fileInfo.Format);
                }
                catch (Exception) // todo: fix pokemon catching
                {
                }
            }
            var writer = new GenericEDIWriter();
            GenericEDIWriter.helper.useLocalTime = bUseLocalTime;
            GenericEDIWriter.helper.LocalTimeZone = localZone ?? TimeZoneInfo.Local;
            var newEDI = writer.CompileTemplate(createTemplate, edi);
            extMapping.PrepareEDIMapping(newEDI);
            foreach (var map in mappings)
            {
                try
                {
                    extMapping.ExecuteEDIMapping(map);
                }
                catch (Exception) // todo: fix pokemon catching
                {
                }
            }
            return extMapping.GetFinalEDIMapping();
        }
    }
}
