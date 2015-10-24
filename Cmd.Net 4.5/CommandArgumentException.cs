using System;
using System.Runtime.Serialization;

namespace Cmd.Net
{
    /// <summary>
    /// The exception that is thrown when one of the arguments provided to a command is not valid.
    /// </summary>
    [Serializable]
    public class CommandArgumentException : CommandException
    {
        #region Fields

        private const string ArgumentNameProperty = "ArgumentName";

        private readonly string _argumentName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandArgumentException" /> class.
        /// </summary>
        public CommandArgumentException()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandArgumentException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected CommandArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _argumentName = info.GetString(ArgumentNameProperty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandArgumentException" /> class
        /// with the specified command name and argument name.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        /// <param name="argumentName">The name of the argument that causes this exception.</param>
        public CommandArgumentException(string commandName, string argumentName)
            : base(commandName)
        {
            _argumentName = argumentName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandArgumentException" /> class
        /// with the specified command name, argument name and error message.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        /// <param name="argumentName">The name of the argument that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public CommandArgumentException(string commandName, string argumentName, string message)
            : base(commandName, message)
        {
            _argumentName = commandName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandArgumentException" /> class
        /// with the specified command name, argument name, error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        /// <param name="argumentName">The name of the argument that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CommandArgumentException(string commandName, string argumentName, string message, Exception innerException)
            : base(commandName, message, innerException)
        {
            _argumentName = argumentName;
        }

        #endregion

        #region Exception Members

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(ArgumentNameProperty, _argumentName);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the argument that causes this exception.
        /// </summary>
        /// <value>The argument name.</value>
        public string ArgumentName
        {
            get { return _argumentName; }
        }

        #endregion
    }
}
