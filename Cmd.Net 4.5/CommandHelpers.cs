using System;

namespace Cmd.Net
{
    internal static class CommandHelpers
    {
        #region Internal Methods

        internal static void ValidateName(string argumentName, string value, bool isNullValid)
        {
            if (!isNullValid)
            {
                if (value == null)
                    throw new ArgumentNullException(argumentName);

                if (value.Length == 0)
                    throw new ArgumentException(null, argumentName);
            }

            if (!IsValidName(value, isNullValid))
                throw new ArgumentException(null, argumentName);
        }

        internal static bool IsValidName(string name, bool isNullValid)
        {
            if (string.IsNullOrEmpty(name))
                return isNullValid;

            for (int i = 0; i < name.Length; i++)
            {
                if (!IsValidNameCharacter(name[i]))
                    return false;
            }

            return true;
        }

        internal static bool IsValidNameCharacter(char c)
        {
            return char.IsLetterOrDigit(c) || c == '-' || c == '_';
        }

        internal static void ValidateFlagName(string argumentName, char value)
        {
            if (!IsValidFlagName(value))
                throw new ArgumentException(null, argumentName);
        }

        internal static bool IsValidFlagName(char c)
        {
            return char.IsLetter(c) && char.IsUpper(c);
        }

        #endregion
    }
}
