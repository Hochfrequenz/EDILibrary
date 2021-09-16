// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using System;
using System.Threading.Tasks;

namespace EDILibrary.Interfaces
{
    public interface ITemplateLoader
    {
        Task<string> LoadEDITemplate(EDIFileInfo info, string type);
        Task<string> LoadJSONTemplate(string fileName);
        /// <summary>
        /// todo @JoschaMetze add docstring
        /// </summary>
        /// <param name="format"></param>
        /// <param name="version">e.g. 5.2h</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<string> LoadJSONTemplate(EdifactFormat? format, string version, string fileName);

        [Obsolete("Use strongly typed overload")]
        Task<string> LoadJSONTemplate(string formatPackage, string fileName);
    }
}
