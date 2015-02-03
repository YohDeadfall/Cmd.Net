using System;

namespace Cmd.Net
{
    /// <summary>
    /// Indicates that input stream should be passed to parameter.
    /// </summary>
    /// <remarks>
    /// You can apply this attribute to parameters of <see cref="T:System.IO.TextReader" /> type.
    /// <para>Combining the <see cref="T:Cmd.Net.InputAttribute" /> and <see cref="T:Cmd.Net.OutputAttribute" /> is particularly useful
    /// when parameter should recieve <see cref="T:System.IO.Stream" /> for read and write operations.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class InputAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.InputAttribute" /> class.
        /// </summary>
        public InputAttribute()
        {
        }

        #endregion
    }
}
