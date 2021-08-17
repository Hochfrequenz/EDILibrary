using System;

namespace EDILibrary.Exceptions
{
    /// <summary>
    /// The exception is thrown if the combination of a edifact format and its version is not supported.
    /// </summary>
    /// <seealso cref="BadPIDException"/>
    public class BadFormatException : Exception
    {
        private string _format;
        private string _version;

        /// <inheritdoc />
        public override string Message => $"Format {_format} in version {_version} could not be found";

        /// <summary>
        /// Initialize the Exception by providing both the (stringly typed) format and the version.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="version"></param>
        public BadFormatException(string format, string version)
        {
            _format = format;
            _version = version;
        }
    }
}
