using System;

namespace EDILibrary.Exceptions
{
    /// <summary>
    /// Exception that is thrown if a pruefidentifikator is not known/not supported (yet).
    /// </summary>
    /// <seealso cref="BadFormatException"/>
    public class BadPIDException : Exception
    {
        private readonly string _pid;

        /// <inheritdoc />
        public override string Message => "Pid " + _pid + " is unknown.";

        /// <summary>
        /// Initialize the exception by providing the unsupported prüfidentifikator / message class <paramref name="pid"/>
        /// </summary>
        /// <param name="pid">e.g. "11099"</param>
        public BadPIDException(string pid)
        {
            _pid = pid;
        }
    }
}
