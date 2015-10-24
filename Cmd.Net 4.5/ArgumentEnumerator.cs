using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cmd.Net
{
    /// <summary>
    /// Supports parsing of command-line arguments and iterating over them.
    /// </summary>
    public sealed class ArgumentEnumerator : IEnumerable<KeyValuePair<string, string>>, IEnumerator<KeyValuePair<string, string>>
    {
        #region Nested Types

        private enum State
        {
            NotSet,
            UsedAsEnumerator,
            Disposed
        }

        #endregion

        #region Fields

        private static ArgumentEnumerator s_empty;

        private State _state;
        private readonly int? _threadID;
        private readonly int _startIndex;
        private string _originalArguments1;
        private string[] _originalArguments2;
        private KeyValuePair<string, string>[] _arguments;
        private KeyValuePair<string, string> _current;
        private int _currentIndex;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.ArgumentEnumerator" /> class with the specified <see cref="T:System.String" />
        /// which represents command-line arguments.
        /// </summary>
        /// <param name="arguments">Command-line arguments to be parsed and iterated.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="arguments" /> is null.</exception>
        public ArgumentEnumerator(string arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            _threadID = Thread.CurrentThread.ManagedThreadId;
            _originalArguments1 = arguments;
            _currentIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.ArgumentEnumerator" /> class with the specified array of <see cref="T:System.String" />
        /// each represents a single command-line argument.
        /// </summary>
        /// <param name="arguments">Command-line arguments to be parsed and iterated.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="arguments" /> is null.</exception>
        public ArgumentEnumerator(params string[] arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            _threadID = Thread.CurrentThread.ManagedThreadId;
            _originalArguments2 = arguments;
            _currentIndex = -1;
        }

        private ArgumentEnumerator()
        {
            _arguments = new KeyValuePair<string, string>[0];
            _currentIndex = -1;
        }

        private ArgumentEnumerator(ArgumentEnumerator enumerator, int startIndex)
        {
            enumerator.ParseArguments();

            _threadID = Thread.CurrentThread.ManagedThreadId;
            _originalArguments1 = enumerator._originalArguments1;
            _originalArguments2 = enumerator._originalArguments2;
            _arguments = enumerator._arguments;
            _startIndex = startIndex;
            _currentIndex = startIndex - 1;
        }

        #endregion

        #region IEnumerable<KeyValuePair<string, string>> Members

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerator<KeyValuePair<string, string>> Members

        /// <summary>
        /// Gets the current argument name/value pair in the collection. 
        /// </summary>
        /// <returns>The current argument name/value in the collection.</returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:Cmd.Net.ArgumentEnumerator" /> has been disposed.</exception>
        public KeyValuePair<string, string> Current
        {
            get
            {
                ThrowIfDisposed();

                if (_currentIndex < _startIndex || _currentIndex >= _arguments.Length)
                    throw new InvalidOperationException();

                return _current;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="T:Cmd.Net.ArgumentEnumerator" /> class.
        /// </summary>
        public void Dispose()
        {
            if (_state != State.Disposed && s_empty != this)
            {
                _state = State.Disposed;
                _originalArguments1 = null;
                _originalArguments2 = null;
                _arguments = null;
                _current = default(KeyValuePair<string, string>);

                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:Cmd.Net.ArgumentEnumerator" /> has been disposed.</exception>
        public bool MoveNext()
        {
            ThrowIfDisposed();
            ParseArguments();

            if (_currentIndex == _arguments.Length)
                return false;

            _currentIndex++;

            if (_currentIndex == _arguments.Length)
            {
                _current = new KeyValuePair<string, string>();
                return false;
            }
            else
            {
                _current = _arguments[_currentIndex];
                return true;
            }
        }

        /// <summary>
        /// Sets the enumerator to its initial position which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:Cmd.Net.ArgumentEnumerator" /> has been disposed.</exception>
        public void Reset()
        {
            ThrowIfDisposed();

            _current = new KeyValuePair<string, string>();
            _currentIndex = _startIndex - 1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an empty <see cref="T:Cmd.Net.ArgumentEnumerator" />.
        /// </summary>
        /// <value>An empty <see cref="T:Cmd.Net.ArgumentEnumerator" />.</value>
        public static ArgumentEnumerator Empty
        {
            get { return s_empty ?? (s_empty = new ArgumentEnumerator()); }
        }

        /// <summary>
        /// Gets the current argument name in the collection. 
        /// </summary>
        /// <value>The current argument name in the collection or an empty string if the argument has no name.</value>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:Cmd.Net.ArgumentEnumerator" /> has been disposed.</exception>
        public string CurrentName
        {
            get { return Current.Key; }
        }

        /// <summary>
        /// Gets the current argument value in the collection. 
        /// </summary>
        /// <value>The current argument value in the collection.</value>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:Cmd.Net.ArgumentEnumerator" /> has been disposed.</exception>
        /// <remarks>
        /// If current command-line argument has value it will be returned. Otherwise either <see cref="F:System.Boolean.TrueString" /> will be returned
        /// if there is no the minus sign before argument name or <see cref="F:System.Boolean.FalseString" /> if the minus sign is being before the name.
        /// </remarks>
        public string CurrentValue
        {
            get { return Current.Value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a continuation that enumerates arguments starting from current.
        /// </summary>
        /// <returns>A new continuation <see cref="T:Cmd.Net.ArgumentEnumerator" />.</returns>
        public ArgumentEnumerator ContinueFromCurrent()
        {
            ThrowIfDisposed();

            if (s_empty == this)
                return this;

            if (_currentIndex == _arguments.Length)
                return Empty;

            return new ArgumentEnumerator(this, _currentIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:Cmd.Net.ArgumentEnumerator" /> that can be used to iterate through the collection.</returns>
        public ArgumentEnumerator GetEnumerator()
        {
            ThrowIfDisposed();

            if (s_empty == this)
            {
                return this;
            }

            if (_state == State.NotSet && _threadID == Thread.CurrentThread.ManagedThreadId)
            {
                _state = State.UsedAsEnumerator;
                return this;
            }

            return new ArgumentEnumerator(this, _startIndex);
        }

        #endregion

        #region Private Methods

        private static string ReadValue(string argument, int startIndex, out int endIndex, bool treatTailAsValue)
        {
            if (treatTailAsValue)
            {
                endIndex = argument.Length;
                return argument.Substring(startIndex);
            }

            int endIndexLocal;

            if (argument[startIndex] == '"')
            {
                startIndex++;
                endIndex = startIndex;

                while (true)
                {
                    if (endIndex == argument.Length)
                        throw new InvalidOperationException();

                    if (argument[endIndex] == '"' && (++endIndex == argument.Length || argument[endIndex] != '"'))
                        break;
                    else
                        endIndex++;
                }

                endIndexLocal = endIndex - 1;
            }
            else
            {
                for (
                    endIndex = startIndex + 1;
                    endIndex < argument.Length && !char.IsWhiteSpace(argument[endIndex]);
                    endIndex++
                    ) ;

                endIndexLocal = endIndex;
            }

            return argument.Substring(startIndex, endIndexLocal - startIndex);
        }

        private static KeyValuePair<string, string> ParseArgument(string argument, int startIndex, out int endIndex, bool treatTailAsValue)
        {
            if (argument[startIndex] == '/')
            {
                int delimiterIndex = -1;
                char c = '\0';

                startIndex++;

                for (
                    endIndex = startIndex + 1;
                    endIndex < argument.Length && (CommandHelpers.IsValidNameCharacter(c = argument[endIndex]));
                    endIndex++
                    ) ;

                if (c == ':')
                    delimiterIndex = endIndex;

                if (argument[startIndex] == '-')
                {
                    startIndex++;

                    if (delimiterIndex != -1)
                        throw new InvalidOperationException();

                    return new KeyValuePair<string, string>(
                        argument.Substring(startIndex, endIndex - startIndex),
                        bool.FalseString
                        );
                }

                if (delimiterIndex == -1)
                {
                    return new KeyValuePair<string, string>(
                        argument.Substring(startIndex, endIndex - startIndex),
                        bool.TrueString
                        );
                }
                else
                {
                    return new KeyValuePair<string, string>(
                        argument.Substring(startIndex, delimiterIndex - startIndex),
                        ReadValue(argument, delimiterIndex + 1, out endIndex, treatTailAsValue)
                        );
                }
            }

            return new KeyValuePair<string, string>(
                string.Empty,
                ReadValue(argument, startIndex, out endIndex, treatTailAsValue)
                );
        }

        private static KeyValuePair<string, string>[] ParseArguments(string arguments)
        {
            LinkedList<KeyValuePair<string, string>> result = null;
            char c = '\0';
            int startIndex;
            int endIndex = 0;

            while (endIndex < arguments.Length)
            {
                for (
                    startIndex = endIndex;
                    startIndex < arguments.Length && (c = arguments[startIndex]) != '"' && char.IsWhiteSpace(c);
                    startIndex++
                    ) ;

                if (startIndex == arguments.Length)
                    break;

                if (result == null)
                    result = new LinkedList<KeyValuePair<string, string>>();

                result.AddLast(ParseArgument(arguments, startIndex, out endIndex, false));
            }

            return (result == null)
                ? new KeyValuePair<string, string>[0]
                : result.ToArray();
        }

        private static KeyValuePair<string, string>[] ParseArguments(string[] arguments)
        {
            KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[arguments.Length];
            int endIndex;

            for (int i = 0; i < arguments.Length; i++)
                result[i] = ParseArgument(arguments[i], 0, out endIndex, true);

            return result;
        }

        private void ParseArguments()
        {
            if (_arguments == null)
            {
                _arguments = (_originalArguments1 == null)
                    ? ParseArguments(_originalArguments2)
                    : ParseArguments(_originalArguments1);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_state == State.Disposed)
                throw new ObjectDisposedException(null);
        }

        #endregion
    }
}
