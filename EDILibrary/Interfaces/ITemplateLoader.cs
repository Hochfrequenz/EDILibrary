// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using System;
using System.Threading.Tasks;

namespace EDILibrary.Interfaces
{
    public interface ITemplateLoader
    {
        Task<string> LoadEDITemplate(EDIFileInfo info, string type);
        Task<string> LoadJSONTemplate(string fileName);
        Task<string> LoadJSONTemplate(EdifactFormatVersion formatPackage, string fileName);

        [Obsolete("Use strongly typed overload")]
        Task<string> LoadJSONTemplate(string formatPackage, string fileName);
    }
}
