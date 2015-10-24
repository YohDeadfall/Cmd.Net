using System;

namespace Cmd.Net
{
    /// <summary>
    /// Specifies a name for an argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ArgumentAttribute : Attribute
    {
        #region Fields

        private readonly string _name;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.ArgumentAttribute" /> class with the default argument name which is an empty string ("").
        /// </summary>
        public ArgumentAttribute()
        {
            _name = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.ArgumentAttribute" /> class using the specified argument name.
        /// </summary>
        /// <param name="name">The name for an argument.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> contains one or more invalid characters.</exception>
        /// <remarks>
        /// A <paramref name="name" /> can be null, an empty string (""), or can contain letters, digits and underscore characters.
        /// </remarks>
        public ArgumentAttribute(string name)
        {
            CommandHelpers.ValidateName("name", name, true);
            _name = name ?? string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name stored in this attribute.
        /// </summary>
        /// <value>The name stored in this attribute.</value>
        public string Name
        {
            get { return _name; }
        }

        #endregion
    }
}
