using System;
using System.Linq;
using System.Threading.Tasks;
using EDILibrary.MAUS;

namespace EDILibrary.Interfaces
{
    /// <summary>
    /// Extension methods for any <see cref="ITemplateLoader"/>
    /// </summary>
    public static class TemplateLoaderExtensions
    {
        /// <summary>
        /// Calls the <see cref="ITemplateLoader.LoadMausTemplate"/> for all <see cref="EdifactFormatVersion"/>s younger than <paramref name="formatVersion"/> until a maus is found.
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="format"></param>
        /// <param name="formatVersion"></param>
        /// <param name="pid"></param>
        /// <returns>
        /// The MAUS matching the params or any maus younger than <paramref name="formatVersion"/> plus the actual format version of the loaded maus.
        /// The latter can be used to e.g. log a warning in the application if the requested maus was not found and a fallback is used.
        /// </returns>
        public static async Task<
            Tuple<Anwendungshandbuch, EdifactFormatVersion?>
        > LoadMausTemplateOrFallback(
            this ITemplateLoader loader,
            EdifactFormat format,
            EdifactFormatVersion formatVersion,
            string pid
        )
        {
            var originalRequestedTemplate = await loader.LoadMausTemplate(
                format,
                formatVersion,
                pid
            );
            if (originalRequestedTemplate is not null)
            {
                return new Tuple<Anwendungshandbuch, EdifactFormatVersion?>(
                    originalRequestedTemplate,
                    formatVersion
                );
            }
            foreach (
                var olderFormatVersion in Enum.GetValues<EdifactFormatVersion>()
                    .Where(fv => fv < formatVersion)
                    .OrderByDescending(fv => fv)
            )
            {
                var olderMaus = await loader.LoadMausTemplate(format, olderFormatVersion, pid);
                if (olderMaus != null)
                {
                    // Using the {PID} maus from format version {olderFormatVersion} as fallback for the requested {formatVersion} (DEV-20190), pid, olderFormatVersion, formatVersion);
                    return new Tuple<Anwendungshandbuch, EdifactFormatVersion?>(
                        olderMaus,
                        olderFormatVersion
                    );
                }

                if (olderFormatVersion == EdifactFormatVersion.FV1710)
                {
                    // No fallback found. This is not necessarily a problem.
                    // No maus and no fallback found for {Pid}, {FormatVersion}
                }
            }
            return new Tuple<Anwendungshandbuch, EdifactFormatVersion?>(null, null);
        }
    }
}
