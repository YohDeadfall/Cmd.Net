using System;
using System.IO;

namespace Cmd.Net
{
    /// <summary>
    /// Represents a command that performs an action.
    /// </summary>
    public abstract class Command
    {
        #region Fields

        private readonly string _name;
        private readonly string _description;

        internal const int ArgumentIndent = 4;
        internal const int EnumerationIndent = 8;
        internal const int DescriptionIndentMax = 24;
        internal const int DescriptionGap = 2;
        internal const int ExampleIndent = 4;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.Command" /> class using the specified command name.
        /// </summary>
        /// <param name="name">The name for a command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> is an empty string (""), or contains one or more invalid characters.</exception>
        /// <remarks>
        /// A <paramref name="name" /> can contain letters, digits and underscore characters.
        /// </remarks>
        protected Command(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.Command" /> class using the specified command name and description.
        /// </summary>
        /// <param name="name">The name of a command.</param>
        /// <param name="description">The description of a command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> is an empty string (""), or contains one or more invalid characters.</exception>
        /// <remarks>
        /// A <paramref name="name" /> can contain letters, digits and underscore characters.
        /// </remarks>
        protected Command(string name, string description)
        {
            CommandHelpers.ValidateName("name", name, false);

            _name = name;
            _description = description;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        /// <value>The description of the command.</value>
        public string Description
        {
            get { return _description; }
        }

        #endregion

        #region Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public static implicit operator Command(Delegate method)
        {
            return new DelegateCommand(method);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes a command using standard IO streams and arguments which will be read from the input stream.
        /// </summary>
        public void Execute()
        {
            Execute(Console.In, Console.Out, Console.Error);
        }

        /// <summary>
        /// Executes a command using standard IO streams and specified arguments.
        /// </summary>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        public void Execute(ArgumentEnumerator args)
        {
            Execute(Console.In, Console.Out, Console.Error, args);
        }

        /// <summary>
        /// Executes a command using standard IO streams and specified arguments.
        /// </summary>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        public void Execute(params string[] args)
        {
            Execute(Console.In, Console.Out, Console.Error, args);
        }

        /// <summary>
        /// Executes a command using standard IO streams and specified arguments.
        /// </summary>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        public void Execute(string args)
        {
            Execute(Console.In, Console.Out, Console.Error, args);
        }

        /// <summary>
        /// Executes a command using specified IO streams and arguments which will be read from the input stream.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="input" />, <paramref name="output" />
        /// is null.</exception>
        public void Execute(TextReader input, TextWriter output)
        {
            Execute(input, output, output);
        }

        /// <summary>
        /// Executes a command using specified IO streams and arguments.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="input" />, <paramref name="output" />,
        /// or <paramref name="args" /> is null.</exception>
        public void Execute(TextReader input, TextWriter output, ArgumentEnumerator args)
        {
            Execute(input, output, output, args);
        }

        /// <summary>
        /// Executes a command using specified IO streams and arguments.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="input" />, <paramref name="output" />
        /// is null.</exception>
        public void Execute(TextReader input, TextWriter output, params string[] args)
        {
            Execute(input, output, output, args);
        }

        /// <summary>
        /// Executes a command using specified IO streams and arguments.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="input" />, <paramref name="output" />
        /// is null.</exception>
        public void Execute(TextReader input, TextWriter output, string args)
        {
            Execute(input, output, output, args);
        }

        /// <summary>
        /// Executes a command using specified IO streams and arguments which will be read from the input stream.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="error">The <see cref="T:System.IO.TextWriter" /> that represents an error stream.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="input" />, <paramref name="output" />,
        /// or <paramref name="error" /> is null.</exception>
        public void Execute(TextReader input, TextWriter output, TextWriter error)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (output == null)
                throw new ArgumentNullException("output");

            if (error == null)
                throw new ArgumentNullException("error");

            string arguments = input.ReadLine();

            ExecuteCore(
                input,
                output,
                error,
                (string.IsNullOrEmpty(arguments))
                    ? ArgumentEnumerator.Empty
                    : new ArgumentEnumerator(arguments)
                );
        }

        /// <summary>
        /// Executes a command using specified IO streams and arguments.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="error">The <see cref="T:System.IO.TextWriter" /> that represents an error stream.</param>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="input" />, <paramref name="output" />,
        /// <paramref name="error" />, or <paramref name="args" /> is null.</exception>
        public void Execute(TextReader input, TextWriter output, TextWriter error, ArgumentEnumerator args)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (output == null)
                throw new ArgumentNullException("output");

            if (error == null)
                throw new ArgumentNullException("error");

            if (args == null)
                throw new ArgumentNullException("args");

            ExecuteCore(input, output, error, args);
        }

        /// <summary>
        /// Executes a command using specified IO streams and arguments.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="error">The <see cref="T:System.IO.TextWriter" /> that represents an error stream.</param>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="input" />, <paramref name="output" />,
        /// or <paramref name="error" /> is null.</exception>
        public void Execute(TextReader input, TextWriter output, TextWriter error, params string[] args)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (output == null)
                throw new ArgumentNullException("output");

            if (error == null)
                throw new ArgumentNullException("error");

            ExecuteCore(
                input,
                output,
                error,
                (args == null || args.Length == 0)
                    ? ArgumentEnumerator.Empty
                    : new ArgumentEnumerator(args)
                );
        }

        /// <summary>
        /// Executes a command using specified IO streams and arguments.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="error">The <see cref="T:System.IO.TextWriter" /> that represents an error stream.</param>
        /// <param name="args">Command-line arguments to be passed to the command.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="input" />, <paramref name="output" />,
        /// or <paramref name="error" /> is null.</exception>
        public void Execute(TextReader input, TextWriter output, TextWriter error, string args)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (output == null)
                throw new ArgumentNullException("output");

            if (error == null)
                throw new ArgumentNullException("error");

            ExecuteCore(
                input,
                output,
                error,
                (string.IsNullOrEmpty(args))
                    ? ArgumentEnumerator.Empty
                    : new ArgumentEnumerator(args)
                );
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// When overridden in a derived class, provides execution logic.
        /// </summary>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="error">The <see cref="T:System.IO.TextWriter" /> that represents an error stream.</param>
        /// <param name="args">Command-line arguments which were passed to the command.</param>
        protected abstract void ExecuteCore(TextReader input, TextWriter output, TextWriter error, ArgumentEnumerator args);

        #endregion
    }
}
