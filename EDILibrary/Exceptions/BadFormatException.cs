using System;

namespace EDILibrary.Exceptions
{
    /// <summary>
    /// The exception is thrown if the combination of a edifact format and its version is not supported.
    /// </summary>
    /// <seealso cref="BadPIDException"/>
    public class BadFormatException : Exception
    {
        private EdifactFormat? _format;

        /// <summary>
        /// e.g. 5.2h
        /// </summary>
        private string _version;

        public BadFormatException(EdifactFormat? format, string version, Exception innerException)
            : base($"Format {format} in version {version} could not be found", innerException)
        {
            _format = format;
            _version = version;
        }

        [Obsolete("Use strongly typed overload instead")]
        public BadFormatException(string format, string version, Exception innerException)
            : this(Enum.Parse<EdifactFormat>(format), version, innerException) { }
    }
}
