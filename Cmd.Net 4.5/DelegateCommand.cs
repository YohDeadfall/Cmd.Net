using Cmd.Net.Properties;
using System;
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

        private readonly Delegate _method;
        private readonly Argument[] _argumentArray;
        private readonly Dictionary<string, Argument> _argumentMap;

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

            _method = method;
            _argumentArray = new Argument[parameterInfos.Length];
            _argumentMap = new Dictionary<string, Argument>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                ParameterInfo parameterInfo = parameterInfos[i];
                Argument argument = new Argument(parameterInfo);

                _argumentArray[i] = argument;
                _argumentMap.Add(argument.ArgumentName, argument);
            }
        }

        #endregion

        #region CommandBase Members

        /// <inheritdoc />
        protected override void ExecuteCore(TextReader input, TextWriter output, TextWriter error, ArgumentEnumerator args)
        {
            if (args.MoveNext() && string.CompareOrdinal(args.CurrentName, "?") == 0)
                ExecuteHelp(output, args);
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

            VerbAttribute verbAttribute = method.Method.GetCustomAttribute<VerbAttribute>();

            return (verbAttribute != null)
                ? verbAttribute.Verb
                : null;
        }

        private static string GetCommandDescription(Delegate method)
        {
            if (method == null)
                return null;

            DescriptionAttribute descriptionAttribute = method.Method.GetCustomAttribute<DescriptionAttribute>();

            return (descriptionAttribute != null)
                ? descriptionAttribute.Description
                : null;
        }

        private void ExecuteHelp(TextWriter output, ArgumentEnumerator args)
        {
            string description = Description;

            if (description != null)
                output.WriteLine(description);

            char namePrefix = args.NamePrefix;

            WriteSyntax(output, namePrefix);
            WriteArguments(output, namePrefix);
            WriteRemarks(output);
            WriteExamples(output, namePrefix);
        }

        private void WriteSyntax(TextWriter output, char namePrefix)
        {
            output.WriteLine();
            output.WriteLine(Resources.SyntaxSection);
            output.WriteLine();

            for (int indent = 0; indent < ArgumentIndent; ++indent)
                output.Write(' ');

            output.Write(Name);

            foreach (Argument argument in _argumentArray)
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
                    output.Write(namePrefix);
                    output.Write(argument.ArgumentName);

                    if (argument.Type != typeof(bool))
                    {
                        output.Write(':');
                        output.Write(argument.ParameterName);
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
        }

        private void WriteArguments(TextWriter output, char namePrefix)
        {
            if (_argumentArray.Length == 0)
                return;

            output.WriteLine();
            output.WriteLine(Resources.AttributesSection);
            output.WriteLine();

            int descriptionIndent = 0;

            foreach (Argument argument in _argumentArray)
            {
                if (argument.IsInput || argument.IsOutput || argument.IsError)
                    continue;

                int nameLength = (argument.IsNamed)
                    ? argument.ArgumentName.Length + 1
                    : argument.ParameterName.Length;

                if (argument.IsEnum && !argument.IsBitField)
                {
                    int valueLengthMax = 0;

                    foreach (FieldInfo value in argument.Type.GetFields(BindingFlags.Public | BindingFlags.Static))
                    {
                        int valueLength = value.Name.Length;

                        if (valueLengthMax < valueLength)
                            valueLengthMax = valueLength;
                    }

                    valueLengthMax += EnumerationIndent - ArgumentIndent;

                    if (nameLength < valueLengthMax)
                        nameLength = valueLengthMax;
                }

                if (descriptionIndent < nameLength)
                    descriptionIndent = nameLength;
            }

            descriptionIndent += ArgumentIndent + DescriptionGap;

            if (descriptionIndent > DescriptionIndentMax)
                descriptionIndent = DescriptionIndentMax;

            for (int argumentIndex = 0; argumentIndex < _argumentArray.Length; ++argumentIndex)
            {
                Argument argument = _argumentArray[argumentIndex];

                if (argument.IsInput || argument.IsOutput || argument.IsError)
                    continue;

                if (argumentIndex > 0)
                    output.WriteLine();

                int indent;

                for (indent = 0; indent < ArgumentIndent; ++indent)
                    output.Write(' ');

                if (argument.IsNamed)
                {
                    indent += argument.ArgumentName.Length + 1;

                    output.Write(namePrefix);
                    output.Write(argument.ArgumentName);
                }
                else
                {
                    indent += argument.ParameterName.Length;

                    output.Write(argument.ParameterName);
                }

                if (indent + DescriptionGap > descriptionIndent)
                {
                    indent = 0;
                    output.WriteLine();
                }

                for (; indent < descriptionIndent; ++indent)
                    output.Write(' ');

                output.WriteIndented(argument.Description, indent, false);

                if (argument.IsEnum)
                {
                    foreach (FieldInfo value in argument.Type.GetFields(BindingFlags.Public | BindingFlags.Static))
                    {
                        string name = null;
                        string description = null;

                        if (argument.IsBitField)
                        {
                            FlagAttribute fa = value.GetCustomAttribute<FlagAttribute>();

                            if (fa == null)
                                continue;

                            name = new string(fa.Name, 1);
                        }
                        else
                        {
                            name = value.Name;
                        }

                        DescriptionAttribute da = value.GetCustomAttribute<DescriptionAttribute>();

                        if (da != null)
                            description = da.Description;

                        output.WriteLine();

                        for (indent = 0; indent < EnumerationIndent; ++indent)
                            output.Write(' ');

                        output.Write(name);

                        indent += name.Length;

                        if (indent + DescriptionGap > descriptionIndent)
                        {
                            indent = 0;
                            output.WriteLine();
                        }

                        for (; indent < descriptionIndent; ++indent)
                            output.Write(' ');

                        output.WriteIndented(description, indent, false);
                    }
                }
            }

            output.WriteLine();
        }

        private void WriteRemarks(TextWriter output)
        {
            RemarksAttribute ra = _method.Method.GetCustomAttribute<RemarksAttribute>();

            if (ra != null)
            {
                output.WriteLine();
                output.WriteLine(ra.Remarks);
            }
        }

        private void WriteExamples(TextWriter output, char namePrefix)
        {
            Attribute[] eas = Attribute.GetCustomAttributes(_method.Method, typeof(ExampleAttribute));

            if (eas.Length == 0)
                return;

            output.WriteLine();
            output.WriteLine(Resources.ExamplesSection);

            foreach (ExampleAttribute ea in eas)
            {
                output.WriteLine();

                string description = ea.Description;

                if (description != null && description.Length > 0)
                {
                    output.WriteLine(description);
                    output.WriteLine();
                }

                output.WriteIndent(ExampleIndent);
                output.Write(Name);

                ArgumentEnumerator args = new ArgumentEnumerator(ea.NamePrefix, ea.Example);

                while (args.MoveNext())
                {
                    output.Write(' ');
                    output.Write(namePrefix);
                    output.Write(args.CurrentName);

                    if (args.CurrentValue != bool.FalseString &&
                        args.CurrentValue != bool.TrueString)
                    {
                        output.Write(':');
                        output.Write(args.CurrentValue);
                    }
                }

                output.WriteLine();
            }
        }

        private void ExecuteMethod(TextReader input, TextWriter output, TextWriter error, ArgumentEnumerator args)
        {
            args.Reset();

            object[] parameterValues = ParseArguments(args, error);
            Stream inOutStream = null;

            foreach (Argument argument in _argumentMap.Values)
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
            object[] parameterValues = new object[_argumentMap.Count];

            for (int i = 0; i < parameterValues.Length; i++)
                parameterValues[i] = Type.Missing;

            foreach (IGrouping<string, string> argumentValue in args.GroupBy((i) => i.Key, (i) => i.Value))
            {
                Argument argument;

                if (!_argumentMap.TryGetValue(argumentValue.Key, out argument))
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

            foreach (Argument argument in _argumentMap.Values)
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
