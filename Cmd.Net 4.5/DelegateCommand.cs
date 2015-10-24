using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cmd.Net
{
    /// <summary>
    /// Implements a <see cref="T:Cmd.Net.Command" /> that executes a delegate.
    /// </summary>
    public sealed class DelegateCommand : Command
    {
        #region Nested Types

        private sealed class InOutStream : Stream
        {
            #region Fields

            private readonly TextReader _reader;
            private readonly TextWriter _writer;
            private readonly Encoder _encoder;
            private readonly Decoder _decoder;
            private readonly char[] _bufferRead;
            private readonly char[] _bufferWrite;
            private int _bufferReadPosition;
            private int _bufferReadLength;

            #endregion

            #region Constructors

            private InOutStream(TextReader reader, TextWriter writer)
            {
                StreamReader streamReader = reader as StreamReader;
                StreamWriter streamWriter = writer as StreamWriter;

                _reader = reader;
                _writer = writer;
                _encoder = (streamReader == null)
                    ? Encoding.Default.GetEncoder()
                    : streamReader.CurrentEncoding.GetEncoder();
                _decoder = (streamWriter == null)
                    ? Encoding.Default.GetDecoder()
                    : streamWriter.Encoding.GetDecoder();
                _bufferRead = new char[256];
                _bufferWrite = new char[256];
            }

            #endregion

            #region Stream Members

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override long Length
            {
                get { throw new NotSupportedException("UnseekableStream"); }
            }

            public override long Position
            {
                get { throw new NotSupportedException("UnseekableStream"); }
                set { throw new NotSupportedException("UnseekableStream"); }
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (buffer == null)
                    throw new ArgumentNullException("buffer");

                if (offset < 0 || offset >= buffer.Length)
                    throw new ArgumentException("offset");

                if (count < 0 || offset + count > buffer.Length)
                    throw new ArgumentOutOfRangeException("count");

                int readBytes = 0;

                while (count > 0)
                {
                    int byteCount;
                    int charCount = _bufferReadLength;

                    if (_bufferReadLength == 0)
                    {
                        _bufferReadLength = _reader.Read(_bufferRead, 0, _bufferRead.Length);

                        if (_bufferReadLength == 0)
                            return readBytes;
                    }

                    for (
                        byteCount = _encoder.GetByteCount(_bufferRead, _bufferReadPosition, charCount, false);
                        byteCount > count;
                        byteCount = _encoder.GetByteCount(_bufferRead, _bufferReadPosition, charCount--, false)
                        ) ;

                    _encoder.GetBytes(_bufferRead, _bufferReadPosition, charCount, buffer, offset, false);

                    _bufferReadPosition += charCount;
                    _bufferReadLength -= charCount;
                    offset += byteCount;
                    count -= byteCount;
                    readBytes += byteCount;
                }

                return readBytes;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException("UnseekableStream");
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException("UnseekableStream");
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (buffer == null)
                    throw new ArgumentNullException("buffer");

                if (offset < 0 || offset >= buffer.Length)
                    throw new ArgumentException("offset");

                if (count < 0 || offset + count > buffer.Length)
                    throw new ArgumentOutOfRangeException("count");

                while (count > 0)
                {
                    int byteCount = count;
                    int charCount;

                    for (
                        charCount = _decoder.GetCharCount(buffer, offset, byteCount, false);
                        charCount > _bufferWrite.Length;
                        charCount = _decoder.GetCharCount(buffer, offset, byteCount--, false)
                        ) ;

                    _decoder.GetChars(buffer, offset, count, _bufferWrite, charCount, false);
                    _writer.Write(_bufferWrite, 0, charCount);

                    offset += byteCount;
                    count -= byteCount;
                }
            }

            #endregion

            #region Internal Methods

            internal static Stream Create(TextReader reader, TextWriter writer)
            {
                StreamReader streamReader = reader as StreamReader;
                StreamWriter streamWriter = writer as StreamWriter;

                return (streamReader != null && streamWriter != null && streamReader.BaseStream == streamWriter.BaseStream)
                    ? streamReader.BaseStream
                    : new InOutStream(reader, writer);
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly Dictionary<string, Argument> _arguments;
        private readonly Delegate _method;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.DelegateCommand" /> class using
        /// the specified method which will be invoked.
        /// </summary>
        /// <param name="method">The <see cref="T:System.Delegate" /> to be invoked.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="method" /> is null.</exception>
        public DelegateCommand(Delegate method)
            : this(GetCommandName(method), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.DelegateCommand" /> class using
        /// the specified name, and method which will be invoked.
        /// </summary>
        /// <param name="name">The name of a command.</param>
        /// <param name="method">The <see cref="T:System.Delegate" /> to be invoked.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name" /> or <paramref name="method" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> is an empty string (""), or contains one or more invalid characters.</exception>
        /// <remarks>
        /// A <paramref name="name" /> can contain letters, digits and underscore characters.
        /// </remarks>
        public DelegateCommand(string name, Delegate method)
            : this(name, GetCommandDescription(method), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cmd.Net.DelegateCommand" /> class using
        /// the specified name, description, and method which will be invoked.
        /// </summary>
        /// <param name="name">The name of a command.</param>
        /// <param name="description">The description of a command.</param>
        /// <param name="method">The <see cref="T:System.Delegate" /> to be invoked.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name" /> or <paramref name="method" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> is an empty string (""), or contains one or more invalid characters.</exception>
        /// <remarks>
        /// A <paramref name="name" /> can contain letters, digits and underscore characters.
        /// </remarks>
        public DelegateCommand(string name, string description, Delegate method)
            : base(name, description)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            MethodInfo methodInfo = method.Method;
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();

            _arguments = new Dictionary<string, Argument>(StringComparer.OrdinalIgnoreCase);
            _method = method;

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            {
                Argument argument = new Argument(parameterInfo);
                _arguments.Add(argument.ArgumentName, argument);
            }
        }

        #endregion

        #region CommandBase Members

        /// <inheritdoc />
        protected override void ExecuteCore(TextReader input, TextWriter output, TextWriter error, ArgumentEnumerator args)
        {
            if (args.MoveNext() && string.CompareOrdinal(args.CurrentName, "?") == 0)
                ExecuteHelp(output);
            else
                ExecuteMethod(input, output, error, args);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="T:System.Delegate" /> to be invoked.
        /// </summary>
        /// <value>The <see cref="T:System.Delegate" /> that represents the invoked method.</value>
        public Delegate Method
        {
            get { return _method; }
        }

        #endregion

        #region Private Methods

        private static string GetCommandName(Delegate method)
        {
            if (method == null)
                return null;

            VerbAttribute verbAttribute = (VerbAttribute)method.Method
                .GetCustomAttributes(typeof(VerbAttribute), true)
                .FirstOrDefault();

            return (verbAttribute != null)
                ? verbAttribute.Verb
                : null;
        }

        private static string GetCommandDescription(Delegate method)
        {
            if (method == null)
                return null;

            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)method.Method
                .GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault();

            return (descriptionAttribute != null)
                ? descriptionAttribute.Description
                : null;
        }

        private void ExecuteHelp(TextWriter output)
        {
            string description = Description;

            if (description != null)
            {
                output.WriteLine(description);
                output.WriteLine();
            }

            output.Write('/');
            output.Write(Name);

            IEnumerable<Argument> orderedArguments = (_arguments.Count > 0)
                ? _arguments.Values.OrderBy((a) => a.Position).ToArray()
                : Enumerable.Empty<Argument>();

            foreach (Argument argument in orderedArguments)
            {
                if (argument.IsInput || argument.IsOutput || argument.IsError)
                    continue;

                output.Write(' ');

                if (!argument.IsRequired)
                {
                    output.Write('[');
                }

                if (argument.ArgumentName.Length == 0)
                {
                    output.Write(argument.ParameterName);
                }
                else
                {
                    output.Write('/');
                    output.Write(argument.ArgumentName);

                    if (argument.Type != typeof(bool))
                    {
                        output.Write(':');

                        if (argument.Type.IsEnum)
                        {
                            output.Write('{');

                            IEnumerator nameEnumerator = Enum
                                .GetNames(argument.Type)
                                .GetEnumerator();
                            bool nameEnumeratorMoveNext = nameEnumerator.MoveNext();

                            while (nameEnumeratorMoveNext)
                            {
                                output.Write(nameEnumerator.Current);
                                nameEnumeratorMoveNext = nameEnumerator.MoveNext();

                                if (nameEnumeratorMoveNext)
                                    output.Write('|');
                            }

                            output.Write('}');
                        }
                        else
                        {
                            output.Write("value");
                        }
                    }
                }

                if (argument.AreMultipleAllowed)
                {
                    output.Write(' ');
                    output.Write('[');

                    if (argument.ArgumentName.Length == 0)
                        output.Write(argument.ParameterName);
                    else
                        output.Write(argument.ArgumentName);

                    output.Write(']');
                }

                if (!argument.IsRequired)
                    output.Write(']');
            }

            output.WriteLine();
            output.WriteLine();

            int argumentNameMaxLength = 0;

            if (_arguments.Count == 0)
            {
                return;
            }

            foreach (Argument argument in orderedArguments)
            {
                if (argument.IsInput || argument.IsOutput || argument.IsError)
                    continue;

                int argumentNameLength = (argument.ArgumentName.Length == 0)
                    ? argument.ParameterName.Length
                    : argument.ArgumentName.Length;

                if (argumentNameLength > argumentNameMaxLength)
                    argumentNameMaxLength = argumentNameLength;
            }

            foreach (Argument argument in orderedArguments)
            {
                if (argument.IsInput || argument.IsOutput || argument.IsError)
                    continue;

                int argumentNameLength = argument.ArgumentName.Length;

                output.Write(' ');
                output.Write(' ');

                if (argumentNameLength == 0)
                {
                    argumentNameLength = argument.ParameterName.Length;

                    output.Write(argument.ParameterName);
                    output.Write(' ');
                }
                else
                {
                    output.Write('/');
                    output.Write(argument.ArgumentName);
                }


                if (argumentNameLength < 12)
                {
                    for (int i = argumentNameMaxLength - argumentNameLength; i >= 0; i--)
                        output.Write(' ');
                }
                else
                {
                    output.WriteLine();

                    for (int i = argumentNameMaxLength; i >= 0; i--)
                        output.Write(' ');
                }

                output.WriteLine(argument.Description);
            }
        }

        private void ExecuteMethod(TextReader input, TextWriter output, TextWriter error, ArgumentEnumerator args)
        {
            args.Reset();

            object[] parameterValues = ParseArguments(args, error);
            Stream inOutStream = null;

            foreach (Argument argument in _arguments.Values)
            {
                if (argument.IsInput && argument.IsOutput)
                {
                    if (inOutStream == null)
                        inOutStream = InOutStream.Create(input, output);

                    parameterValues[argument.Position] = inOutStream;
                }
                else
                {
                    if (argument.IsInput)
                        parameterValues[argument.Position] = input;

                    if (argument.IsOutput)
                        parameterValues[argument.Position] = output;
                }

                if (argument.IsError)
                    parameterValues[argument.Position] = error;
            }

            try
            {
                _method.DynamicInvoke(parameterValues);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex is TargetInvocationException)
                    ex = ex.InnerException;

                if (ex is CommandCanceledException)
                    throw ex;

                ThrowHelper.ThrowCommandExecutionException(Name, ex);
            }
        }

        private object[] ParseArguments(ArgumentEnumerator args, TextWriter error)
        {
            object[] parameterValues = new object[_arguments.Count];

            for (int i = 0; i < parameterValues.Length; i++)
                parameterValues[i] = Type.Missing;

            foreach (IGrouping<string, string> argumentValue in args.GroupBy((i) => i.Key, (i) => i.Value))
            {
                Argument argument;

                if (!_arguments.TryGetValue(argumentValue.Key, out argument))
                    ThrowHelper.ThrowUnknownArgumentException(Name, argumentValue.Key);

                int argumentValueCount = argumentValue.Count();

                if (argument.AreMultipleAllowed)
                {
                    if (argument.Type.IsAssignableFrom(typeof(IEnumerable<string>)))
                    {
                        parameterValues[argument.Position] = argumentValue;
                    }
                    else
                    {
                        try
                        {
                            parameterValues[argument.Position] = argument.TypeConverter.ConvertFrom(argumentValue);
                        }
                        catch (Exception ex)
                        {
                            ThrowHelper.ThrowCanNotParseArgumentValueException(Name, argument.DisplayName, argument.Type, ex);
                        }
                    }
                }
                else
                {
                    if (argumentValueCount > 1)
                        ThrowHelper.ThrowTooManyValuesForArgumentException(Name, argument.DisplayName);

                    if (argument.Type.IsAssignableFrom(typeof(string)))
                    {
                        parameterValues[argument.Position] = argumentValue.First().ToString();
                    }
                    else
                    {
                        try
                        {
                            parameterValues[argument.Position] = argument.TypeConverter.ConvertFrom(argumentValue.First());
                        }
                        catch (Exception ex)
                        {
                            ThrowHelper.ThrowCanNotParseArgumentValueException(Name, argument.DisplayName, argument.Type, ex);
                        }
                    }
                }
            }

            foreach (Argument argument in _arguments.Values)
            {
                if (!(argument.IsInput || argument.IsOutput || argument.IsError) && parameterValues[argument.Position] == Type.Missing)
                {
                    if (argument.IsRequired)
                        ThrowHelper.ThrowRequiredArgumentException(Name, argument.DisplayName);

                    parameterValues[argument.Position] = argument.DefaultValue;
                }
            }

            return parameterValues;
        }

        #endregion
    }
}
