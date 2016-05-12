using System;
using System.IO;

namespace Cmd.Net
{
    /// <summary>
    /// 
    /// </summary>
    public static class TextWriterExtensions
    {
        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="value"></param>
        /// <param name="indent"></param>
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
