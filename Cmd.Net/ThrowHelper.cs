using Cmd.Net.Properties;
using System;
using System.Globalization;

namespace Cmd.Net
{
    internal static class ThrowHelper
    {
        #region Internal Methods

        internal static void ThrowCanNotParseArgumentValueException(string commandName, string argumentName, Type argumentType, Exception innerException)
        {
            throw new CommandArgumentException(
                commandName,
                argumentName,
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.CanNotParseArgumentValueException,
                    argumentName,
                    argumentType),
                innerException
                );
        }

        internal static void ThrowCommandExecutionException(string commandName, Exception innerException)
        {
            throw new CommandException(
                commandName,
                string.Format(
                    Resources.CommandExecutionException,
                    commandName
                    ),
                innerException
                );
        }

        internal static void ThrowNoCommandContextExecutionScope(string commandName)
        {
            throw new CommandException(
                commandName,
                Resources.NoCommandContextExecutionScope
                );
        }

        internal static void ThrowNotCurrentCommandContextException(string commandName)
        {
            throw new CommandException(
                commandName,
                Resources.NotCurrentCommandContextException
                );
        }

        internal static void ThrowRequiredArgumentException(string commandName, string argumentName)
        {
            throw new CommandArgumentException(
                commandName,
                argumentName,
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.RequiredArgumentException,
                    argumentName
                    )
                );
        }

        internal static void ThrowTooManyValuesForArgumentException(string commandName, string argumentName)
        {
            throw new CommandArgumentException(
                commandName,
                argumentName,
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.TooManyValuesForArgumentException,
                    argumentName
                    )
                );
        }

        internal static void ThrowUnknownArgumentException(string commandName, string argumentName)
        {
            throw new CommandArgumentException(
                commandName,
                argumentName,
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.UnknownArgumentException,
                    argumentName,
                    commandName
                    )
                );
        }

        internal static void ThrowUnknownCommandException(string commandName)
        {
            throw new CommandException(
                commandName,
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.UnknownCommandException,
                    commandName
                    )
                );
        }

        #endregion
    }
}
