using System;

namespace Cmd.Net
{
    /// <summary>
    /// Specifies an example for a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExampleAttribute : Attribute
    {
        #region Fields

        private readonly string _example;
        private string _description;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.ExampleAttribute" /> class using the specified example.
        /// </summary>
        /// <param name="example">The example for a command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="example" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="example" /> is an empty string ("").</exception>
        public ExampleAttribute(string example)
            : this(example, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.ExampleAttribute" /> class using the specified example and description.
        /// </summary>
        /// <param name="example">The example for a command.</param>
        /// <param name="description">The description for a example.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="example" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="example" /> is an empty string ("").</exception>
        public ExampleAttribute(string example, string description)
        {
            if (example == null)
                throw new ArgumentNullException("example");

            if (example.Length == 0)
                throw new ArgumentException(null, "example");

            _example = example;
            _description = description;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the example stored in this attribute.
        /// </summary>
        /// <value>The example stored in this attribute.</value>
        public string Example
        {
            get { return _example; }
        }

        /// <summary>
        /// Gets the description stored in this attribute.
        /// </summary>
        /// <value>The description stored in this attribute.</value>
        public virtual string Description
        {
            get { return DescriptionValue; }
        }

        /// <summary>
        /// Gets or sets the string stored as the description.
        /// </summary>
        /// <value>The string stored as the description.</value>
        /// <remarks>
        /// <para>The default implementation of the <see cref="M:Cmd.Net.ExampleAttribute.Description" /> property simply returns this value.</para>
        /// <para>This extra property exists so that you can derive from <see cref="T:Cmd.Net.ExampleAttribute" /> and provide a localized version.
        /// The derived localizable <see cref="T:Cmd.Net.ExampleAttribute" /> will maintain a private Boolean field to indicate if it has been localized.
        /// On the first access to the <see cref="M:Cmd.Net.ExampleAttribute.Description" /> property, it will look up the localized string and store it back
        /// in the DescriptionValue property.</para>
        /// </remarks>
        protected string DescriptionValue
        {
            get { return _description; }
            set { _description = value; }
        }

        #endregion
    }
}
