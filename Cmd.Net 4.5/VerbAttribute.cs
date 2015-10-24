using System;

namespace Cmd.Net
{
    /// <summary>
    /// Specifies a name for a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class VerbAttribute : Attribute
    {
        #region Fields

        private readonly string _verb;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.VerbAttribute" /> class using the specified command name.
        /// </summary>
        /// <param name="verb">The name for a command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="verb" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="verb" /> is an empty string (""), or contains one or more invalid characters.</exception>
        /// <remarks>
        /// A <paramref name="verb" /> can contain letters, digits and underscore characters.
        /// </remarks>
        public VerbAttribute(string verb)
        {
            CommandHelpers.ValidateName("verb", verb, false);
            _verb = verb;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name stored in this attribute.
        /// </summary>
        /// <value>The name stored in this attribute.</value>
        public string Verb
        {
            get { return _verb; }
        }

        #endregion
    }
}
