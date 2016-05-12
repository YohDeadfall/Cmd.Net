using System;
using System.IO;

namespace Cmd.Net
{
    /// <summary>
    /// Provides utility methods that perform common tasks.
    /// </summary>
    public static class TextWriterExtensions
    {
        #region Public Methods

        /// <summary>
        /// Writes a string to the text string or stream with indentation of every line of the string.
        /// </summary>
        /// <param name="output">A <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="value">The string to write.</param>
        /// <param name="indent">The space count to indent.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="output" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="indent" /> is less than zero.</exception>
        /// <remarks>
        /// If <paramref name="value" /> is null, nothing is written to the text stream.
        /// </remarks>
        public static void WriteIndented(this TextWriter output, string value, int indent)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            if (indent < 0)
                throw new ArgumentOutOfRangeException("indent");

            if (value == null)
                return;

            int index = 0;
            int length = value.Length;
            char ch = '\0';

            while (index < length)
            {
                ch = value[index];
                index++;

                if (ch == '\r' || ch == '\n')
                {
                    if (index < length && value[index] == '\n')
                        index++;

                    output.WriteLine();

                    for (int i = 0; i < indent; ++i)
                        output.Write(' ');
                }
                else
                {
                    output.Write(ch);
                }
            }
        }

        #endregion
    }
}
