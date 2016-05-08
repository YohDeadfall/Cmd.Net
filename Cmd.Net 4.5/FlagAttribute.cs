using System;

namespace Cmd.Net
{
    /// <summary>
    /// Specifies a name for an enum value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FlagAttribute : Attribute
    {
        #region Fields

        private readonly char name;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.FlagAttribute" /> class using the specified flag name.
        /// </summary>
        /// <param name="name">The name for an argument.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> contains one or more invalid characters.</exception>
        /// <remarks>
        /// A <paramref name="name" /> can be null, an empty string (""), or can contain letters, digits and underscore characters.
        /// </remarks>
        public FlagAttribute(char name)
        {
            name = char.ToUpperInvariant(name);
            CommandHelpers.ValidateFlagName("name", name);

            this.name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name stored in this attribute.
        /// </summary>
        /// <value>The name stored in this attribute.</value>
        public char Name
        {
            get { return name; }
        }

        #endregion
    }
}
