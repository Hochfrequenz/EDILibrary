// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EDILibrary.Interfaces
{
    public interface ITemplateLoader
    {
        Task<string> LoadEDITemplate(EDIFileInfo info, string type);
        Task<string> LoadJSONTemplate(string fileName);
        Task<string> LoadJSONTemplate(string formatPackage, string fileName);
    }
}
