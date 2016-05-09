using System;

namespace Cmd.Net
{
    /// <summary>
    /// Specifies remarks for a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RemarksAttribute : Attribute
    {
        #region Fields

        private readonly string _remarks;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.RemarksAttribute" /> class with no remarks.
        /// </summary>
        public RemarksAttribute()
        {
            _remarks = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.RemarksAttribute" /> class using the specified remarks.
        /// </summary>
        /// <param name="remarks">The remarks for an command.</param>
        public RemarksAttribute(string remarks)
        {
            _remarks = remarks ?? string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the remarks stored in this attribute.
        /// </summary>
        /// <value>The remarks stored in this attribute.</value>
        public string Remarks
        {
            get { return _remarks; }
        }

        #endregion
    }
}
