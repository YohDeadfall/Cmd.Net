using System;
using System.Collections.Generic;
using System.IO;

namespace Cmd.Net
{
    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for working objects that implement <see cref="T:Cmd.Net.Command" /> instances.
    /// </summary>
    public static class CommandExtensions
    {
        #region Public Methods

        /// <summary>
        /// Continuously executes a command using standard IO streams until the input stream returns null
        /// or a <see cref="T:Cmd.Net.CommandCanceledException" /> was thrown.
        /// </summary>
        /// <param name="command">The <see cref="T:Cmd.Net.Command" /> to execute.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="command" /> is null.</exception>
        /// <remarks>
        /// This method catches all exceptions of type <see cref="T:Cmd.Net.CommandException" /> and writes them
        /// to the standard error stream.
        /// </remarks>
        public static void ExecuteAll(this Command command)
        {
            ExecuteAll(command, Console.In, Console.Out, Console.Error);
        }

        /// <summary>
        /// Continuously executes a command using specified IO streams until the input stream returns null
        /// or a <see cref="T:Cmd.Net.CommandCanceledException" /> was thrown.
        /// </summary>
        /// <param name="command">The <see cref="T:Cmd.Net.Command" /> to execute.</param>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="command" />, <paramref name="input" />,
        /// or <paramref name="output" /> is null.</exception>
        /// <remarks>
        /// The output stream will be used for writing errors.
        /// <para>This method catches all exceptions of type <see cref="T:Cmd.Net.CommandException" /> and writes them
        /// to the standard error stream.</para>
        /// </remarks>
        public static void ExecuteAll(this Command command, TextReader input, TextWriter output)
        {
            ExecuteAll(command, input, output, output);
        }

        /// <summary>
        /// Continuously executes a command using specified IO streams until the input stream returns null
        /// or a <see cref="T:Cmd.Net.CommandCanceledException" /> was thrown.
        /// </summary>
        /// <param name="command">The <see cref="T:Cmd.Net.Command" /> to execute.</param>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="error">The <see cref="T:System.IO.TextWriter" /> that represents an error stream.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="command" />, <paramref name="input" />,
        /// <paramref name="output" />, or <paramref name="error" /> is null.</exception>
        /// <remarks>
        /// This method catches all exceptions of type <see cref="T:Cmd.Net.CommandException" /> and writes them
        /// to the standard error stream.
        /// </remarks>
        public static void ExecuteAll(this Command command, TextReader input, TextWriter output, TextWriter error)
        {
            if (command == null)
            { throw new ArgumentNullException("command"); }

            if (input == null)
            { throw new ArgumentNullException("input"); }

            if (output == null)
            { throw new ArgumentNullException("output"); }

            if (error == null)
            { throw new ArgumentNullException("error"); }

            CommandContext commandContext = command as CommandContext;

            if (commandContext != null)
            { ExecuteContext(commandContext, input, output, output, true); }
            else
            { ExecuteCommand(command, input, output, output, true); }
        }

        /// <summary>
        /// Executes a command using standard IO streams.
        /// </summary>
        /// <param name="command">The <see cref="T:Cmd.Net.Command" /> to execute.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="command" /> is null.</exception>
        /// <remarks>
        /// This method catches all exceptions of type <see cref="T:Cmd.Net.CommandException" /> and writes them
        /// to the standard error stream.
        /// </remarks>
        public static void ExecuteSingle(this Command command)
        {
            ExecuteSingle(command, Console.In, Console.Out, Console.Error);
        }

        /// <summary>
        /// Executes a command using specified IO streams.
        /// </summary>
        /// <param name="command">The <see cref="T:Cmd.Net.Command" /> to execute.</param>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="command" />, <paramref name="input" />,
        /// or <paramref name="output" /> is null.</exception>
        /// <remarks>
        /// The output stream will be used for writing errors.
        /// <para>This method catches all exceptions of type <see cref="T:Cmd.Net.CommandException" /> and writes them
        /// to the standard error stream.</para>
        /// </remarks>
        public static void ExecuteSingle(this Command command, TextReader input, TextWriter output)
        {
            ExecuteSingle(command, input, output, output);
        }

        /// <summary>
        /// Executes a command using specified IO streams.
        /// </summary>
        /// <param name="command">The <see cref="T:Cmd.Net.Command" /> to execute.</param>
        /// <param name="input">The <see cref="T:System.IO.TextReader" /> that represents an input stream.</param>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> that represents an output stream.</param>
        /// <param name="error">The <see cref="T:System.IO.TextWriter" /> that represents an error stream.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="command" />, <paramref name="input" />,
        /// <paramref name="output" />, or <paramref name="error" /> is null.</exception>
        /// <remarks>
        /// This method catches all exceptions of type <see cref="T:Cmd.Net.CommandException" /> and writes them
        /// to the standard error stream.
        /// </remarks>
        public static void ExecuteSingle(this Command command, TextReader input, TextWriter output, TextWriter error)
        {
            if (command == null)
            { throw new ArgumentNullException("command"); }

            if (input == null)
            { throw new ArgumentNullException("input"); }

            if (output == null)
            { throw new ArgumentNullException("output"); }

            if (error == null)
            { throw new ArgumentNullException("error"); }

            CommandContext commandContext = command as CommandContext;

            if (commandContext != null)
            { ExecuteContext(commandContext, input, output, output, false); }
            else
            { ExecuteCommand(command, input, output, output, false); }
        }

        #endregion

        #region Private Methods

        private static void ExecuteCommand(Command command, TextReader input, TextWriter output, TextWriter error, bool continiousExecution)
        {
            do
            {
                output.Write('>');

                try
                {
                    string arguments = input.ReadLine();

                    if (arguments == null)
                    { break; }

                    command.Execute(input, output, error, arguments);
                }
                catch (CommandException ex)
                {
                    if (ex is CommandCanceledException)
                    { break; }

                    Exception exception = ex;

                    do
                    {
                        error.WriteLine(exception.Message);
                        exception = exception.InnerException;
                    }
                    while (exception != null);

                    if (error != output)
                    { error.WriteLine(); }
                }

                output.WriteLine();
            }
            while (continiousExecution);
        }

        private static void ExecuteContext(CommandContext command, TextReader input, TextWriter output, TextWriter error, bool continiousExecution)
        {
            using (CommandContextScope commandContextScope = new CommandContextScope(command))
            {
                do
                {
                    IEnumerator<CommandContext> contextEnumerator = commandContextScope.GetContextEnumerator();
                    bool contextEnumeratorMoveNext = contextEnumerator.MoveNext();

                    while (contextEnumeratorMoveNext)
                    {
                        output.Write(contextEnumerator.Current.Name);
                        contextEnumeratorMoveNext = contextEnumerator.MoveNext();

                        if (contextEnumeratorMoveNext)
                        { output.Write(' '); }
                    }

                    output.Write('>');

                    try
                    {
                        string arguments = input.ReadLine();

                        if (arguments == null)
                        { break; }

                        commandContextScope
                            .CurrentContext
                            .Execute(input, output, error, arguments);
                    }
                    catch (CommandException ex)
                    {
                        if (ex is CommandCanceledException)
                        { break; }

                        Exception exception = ex;

                        do
                        {
                            error.WriteLine(exception.Message);
                            exception = exception.InnerException;
                        }
                        while (exception != null);

                        if (error != output)
                        { error.WriteLine(); }
                    }

                    output.WriteLine();
                }
                while (continiousExecution);
            }
        }

        #endregion
    }
}
