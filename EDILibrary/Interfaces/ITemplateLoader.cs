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
        /// <summary>
        /// Loads a MAUS template file from the storage provider
        /// </summary>
        /// <param name="format">UTLIMD, MSCONS,...</param>
        /// <param name="version">e.g. 5.2h</param>
        /// <param name="pid">The pruefidentifikator (e.g. 11001)</param>
        /// <param name="useOlderVersionsAsFallback">set to true to use older maus versions in case no maus was found for the requested <paramref name="version"/></param>
        /// <returns>the deserialized anwendungshandbuch</returns>
        Task<EDILibrary.MAUS.Anwendungshandbuch> LoadMausTemplate(EdifactFormat? format, EdifactFormatVersion version, string pid, bool useOlderVersionsAsFallback);

        [Obsolete("Use strongly typed overload")]
        Task<string> LoadJSONTemplate(string formatPackage, string fileName);
    }
}
