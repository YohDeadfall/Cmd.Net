using Cmd.Net.Properties;
using System;
using System.Runtime.Serialization;

namespace Cmd.Net
{
    /// <summary>
    /// The exception that is thrown when execution of a command fails.
    /// </summary>
    [Serializable]
    public class CommandException : Exception
    {
        #region Fields

        private const string CommandNameProperty = "CommandName";

        private readonly string _commandName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandException" /> class.
        /// </summary>
        public CommandException()
            : base(Resources.CommandExecutionException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected CommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _commandName = info.GetString(CommandNameProperty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandException" /> class
        /// with the specified command name.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        public CommandException(string commandName)
            : base(Resources.CommandExecutionException)
        {
            _commandName = commandName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandException" /> class
        /// with the specified command name and error message.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public CommandException(string commandName, string message)
            : base(message)
        {
            _commandName = commandName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandException" /> class
        /// with the specified command name, error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CommandException(string commandName, string message, Exception innerException)
            : base(message, innerException)
        {
            _commandName = commandName;
        }

        #endregion

        #region Exception Members

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(CommandNameProperty, _commandName);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the command that causes this exception.
        /// </summary>
        /// <value>The command name.</value>
        public string CommandName
        {
            get { return _commandName; }
        }

        #endregion
    }
}
