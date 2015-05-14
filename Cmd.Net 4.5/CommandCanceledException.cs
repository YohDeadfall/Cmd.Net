using Cmd.Net.Properties;
using System;
using System.Runtime.Serialization;

namespace Cmd.Net
{
    /// <summary>
    /// The exception that is thrown in a command upon cancellation of parsing input
    /// that the <see cref="T:Cmd.Net.CommandContextScope" /> was processing.
    /// </summary>
    [Serializable]
    public class CommandCanceledException : CommandException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandCanceledException" /> class.
        /// </summary>
        public CommandCanceledException()
            : base(null, Resources.CommandCanceledException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandCanceledException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected CommandCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandCanceledException" /> class
        /// with the specified command name.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        public CommandCanceledException(string commandName)
            : base(commandName, Resources.CommandCanceledException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandCanceledException" /> class
        /// with the specified command name and error message.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public CommandCanceledException(string commandName, string message)
            : base(commandName, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandCanceledException" /> class
        /// with the specified command name, error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="commandName">The name of the command that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CommandCanceledException(string commandName, string message, Exception innerException)
            : base(commandName, message, innerException)
        {
        }

        #endregion
    }
}
