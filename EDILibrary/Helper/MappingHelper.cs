// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using EDILibrary.Interfaces;
using System.Threading.Tasks;

namespace EDILibrary.Helper
{
    public class MappingHelper
    {
        public static async Task<string> ExecuteMappings(EdiObject edi, EDIFileInfo fileInfo, List<string> mappings, ITemplateLoader loader, bool bUseLocalTime = true)
        {
            var createTemplate = await loader.LoadEDITemplate(fileInfo, "create.template");
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
