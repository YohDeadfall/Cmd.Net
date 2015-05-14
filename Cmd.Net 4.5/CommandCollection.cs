using System;
using System.Collections.ObjectModel;

namespace Cmd.Net
{
    /// <summary>
    /// Represents an collection of <see cref="T:Cmd.Net.Command" /> child elements.
    /// </summary>
    public sealed class CommandCollection : KeyedCollection<string, Command>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandCollection" /> class.
        /// </summary>
        public CommandCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        #endregion

        #region KeyedCollection<string, CommandBase> Members

        /// <inheritdoc />
        protected override string GetKeyForItem(Command item)
        {
            return item.Name;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the command with the specified name.
        /// </summary>
        /// <returns>true if the <see cref="T:Cmd.Net.CommandCollection" /> contains an command with the specified name; otherwise, false.</returns>
        /// <param name="name">The key of the value to get.</param>
        /// <param name="command">When this method returns, contains the command with the specified key, if the key is found; otherwise, null. This parameter is passed uninitialized.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name" /> is null.</exception>
        public bool TryGetCommand(string name, out Command command)
        {
            return Dictionary.TryGetValue(name, out command);
        }

        #endregion
    }
}
