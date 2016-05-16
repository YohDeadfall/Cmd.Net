using System;

namespace Cmd.Net
{
    /// <summary>
    /// Specifies remarks for a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RemarksAttribute : Attribute
    {
        #region Fields

        private string _remarks;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.RemarksAttribute" /> class with no remarks.
        /// </summary>
        public RemarksAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.RemarksAttribute" /> class using the specified remarks.
        /// </summary>
        /// <param name="remarks">The remarks for an command.</param>
        public RemarksAttribute(string remarks)
        {
            _remarks = remarks;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the remarks stored in this attribute.
        /// </summary>
        /// <value>The remarks stored in this attribute.</value>
        public virtual string Remarks
        {
            get { return _remarks; }
        }

        /// <summary>
        /// Gets or sets the string stored as the remarks.
        /// </summary>
        /// <value>The string stored as the remarks.</value>
        /// <remarks>
        /// <para>The default implementation of the <see cref="M:Cmd.Net.RemarksAttribute.Remarks" /> property simply returns this value.</para>
        /// <para>This extra property exists so that you can derive from <see cref="T:Cmd.Net.RemarksAttribute" /> and provide a localized version.
        /// The derived localizable <see cref="T:Cmd.Net.RemarksAttribute" /> will maintain a private Boolean field to indicate if it has been localized.
        /// On the first access to the <see cref="M:Cmd.Net.RemarksAttribute.Remarks" /> property, it will look up the localized string and store it back
        /// in the RemarksValue property.</para>
        /// </remarks>
        protected string RemarksValue
        {
            get { return _remarks; }
            set { _remarks = value; }
        }

        #endregion
    }
}
