using System;

namespace Cmd.Net
{
    /// <summary>
    /// Indicates that error stream should be passed to parameter.
    /// </summary>
    /// <remarks>
    /// You can apply this attribute to parameters of <see cref="T:System.IO.TextWriter" /> type.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ErrorAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.ErrorAttribute" /> class.
        /// </summary>
        public ErrorAttribute()
        {
        }

        #endregion
    }
}
