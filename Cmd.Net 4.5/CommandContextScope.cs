using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Cmd.Net
{
    /// <summary>
    /// Creates a block within which a navigation through <see cref="T:Cmd.Net.CommandContext" /> is possible.
    /// </summary>
    public sealed class CommandContextScope : IDisposable
    {
        #region Fields

        [ThreadStatic]
        private static CommandContextScope currentScope;
        private readonly CommandContextScope originalScope;
        private readonly Thread thread = Thread.CurrentThread;
        private readonly CommandContext rootContext;
        private readonly List<CommandContext> contexts;
        private bool disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandContextScope" /> class with the specified <see cref="T:Cmd.Net.CommandContext" />.
        /// </summary>
        /// <param name="context">The root command context.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="context" /> is null.</exception>
        public CommandContextScope(CommandContext context)
        {
            if (context == null)
            { throw new ArgumentNullException("context"); }

            this.originalScope = currentScope;
            this.thread = Thread.CurrentThread;
            this.rootContext = context;
            this.contexts = new List<CommandContext>();
            this.contexts.Add(context);

            currentScope = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.CommandContextScope" /> class that uses the specified array of <see cref="T:Cmd.Net.Command" />
        /// and escaped name of the entry point <see cref="T:System.Reflection.Assembly" /> to create a new <see cref="T:Cmd.Net.CommandContext" /> for the scope.
        /// </summary>
        /// <param name="commands">The array of commands to use when creating the scope for a new <see cref="T:Cmd.Net.CommandContext" />.</param>
        public CommandContextScope(params Command[] commands)
            : this(new CommandContext(GetDefaultContextName(), commands))
        {
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Restores the original <see cref="T:Cmd.Net.CommandContext" /> to the active context and recycles the <see cref="T:Cmd.Net.CommandContextScope" /> object.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            { return; }

            if (thread != Thread.CurrentThread)
            { throw new InvalidOperationException("InvalidContextScopeThread"); }

            if (currentScope != this)
            { throw new InvalidOperationException("InterleavedContextScopes"); }

            disposed = true;
            currentScope = originalScope;

            contexts.Clear();
        }

        #endregion

        #region Properties

        internal static CommandContextScope Current
        {
            get { return currentScope; }
        }

        internal CommandContext RootContext
        {
            get { return rootContext; }
        }

        internal CommandContext CurrentContext
        {
            get { return contexts[contexts.Count - 1]; }
        }

        #endregion

        #region Internal Methods

        internal IEnumerator<CommandContext> GetContextEnumerator()
        {
            return contexts.GetEnumerator();
        }

        internal CommandContext PopContext()
        {
            int index = contexts.Count;

            if (index > 1)
            { contexts.RemoveAt(--index); }

            return contexts[--index];
        }

        internal void PushContext(CommandContext context)
        {
            if (context == null)
            { throw new ArgumentNullException("context"); }
            
            contexts.Add(context);
        }

        #endregion

        #region Private Methods

        private static string GetDefaultContextName()
        {
            string entryAssemblyName = Assembly
                .GetEntryAssembly()
                .GetName()
                .Name;
            char[] defaultContextName = null;

            for (int i = 0; i < entryAssemblyName.Length; i++)
            {
                char ch = entryAssemblyName[i];

                if (CommandHelpers.IsValidNameCharacter(ch))
                {
                    if (defaultContextName != null)
                    { defaultContextName[i] = ch; }

                    continue;
                }

                if (defaultContextName == null)
                {
                    defaultContextName = new char[entryAssemblyName.Length];
                    entryAssemblyName.CopyTo(0, defaultContextName, 0, i);
                }

                defaultContextName[i] = '_';
            }

            return (defaultContextName != null)
                ? new string(defaultContextName)
                : entryAssemblyName;
        }

        #endregion
    }
}
