using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using CollectionConverter = Cmd.Net.CollectionConverter;

namespace Cmd.Net
{
    internal sealed class Argument
    {
        #region Nested Types

        [Flags]
        private enum Flags
        {
            NotSet = 0,
            IsRequired = 1,
            AreMultipleAllowed = IsRequired << 1,
            IsInput = AreMultipleAllowed << 1,
            IsOutput = IsInput << 1,
            IsError = IsOutput << 1
        }

        #endregion

        #region Fields

        private readonly string _argumentName;
        private readonly string _parameterName;
        private readonly string _description;
        private readonly int _position;
        private readonly Flags _flags;
        private readonly Type _type;
        private readonly TypeConverter _typeConverter;
        private readonly object _defaultValue;

        #endregion

        #region Constructors

        internal Argument(ParameterInfo parameterInfo)
        {
            Type parameterType = parameterInfo.ParameterType;

            SetFlag(ref _flags, Flags.IsInput, parameterInfo.GetCustomAttributes(typeof(InputAttribute), true).Length > 0);
            SetFlag(ref _flags, Flags.IsOutput, parameterInfo.GetCustomAttributes(typeof(OutputAttribute), true).Length > 0);
            SetFlag(ref _flags, Flags.IsError, parameterInfo.GetCustomAttributes(typeof(ErrorAttribute), true).Length > 0);

            if (IsInput)
            {
                if (IsOutput || IsError)
                {
                    if (IsOutput == IsError)
                        throw new InvalidOperationException();

                    if (!parameterType.IsAssignableFrom(typeof(Stream)))
                        throw new InvalidOperationException();
                }
                else
                {
                    if (!parameterType.IsAssignableFrom(typeof(TextReader)))
                        throw new InvalidOperationException();
                }
            }
            else
            {
                if (IsOutput || IsError)
                {
                    if (IsOutput == IsError)
                        throw new InvalidOperationException();

                    if (!parameterType.IsAssignableFrom(typeof(TextWriter)))
                        throw new InvalidOperationException();
                }
            }

            ArgumentAttribute argumentAttribute = (ArgumentAttribute)parameterInfo
                .GetCustomAttributes(typeof(ArgumentAttribute), true)
                .FirstOrDefault();

            if (IsInput || IsOutput || IsError)
            {
                if (argumentAttribute != null)
                    throw new InvalidOperationException();

                _argumentName = parameterInfo.Name;
                _parameterName = parameterInfo.Name;
                _description = null;
                _position = parameterInfo.Position;

                SetFlag(ref _flags, Flags.IsRequired, true);
                SetFlag(ref _flags, Flags.AreMultipleAllowed, false);

                _type = parameterType;
                _typeConverter = null;
                _defaultValue = null;

                return;
            }

            bool isCollectionArgument = parameterType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(parameterType);
            TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)parameterInfo
                .GetCustomAttributes(typeof(TypeConverterAttribute), true)
                .FirstOrDefault();
            TypeConverter typeConverter = null;

            if (typeConverterAttribute != null && !string.IsNullOrEmpty(typeConverterAttribute.ConverterTypeName))
            {
                Type typeConverterType = Type.GetType(typeConverterAttribute.ConverterTypeName);

                if (typeConverterType != null && typeof(TypeConverter).IsAssignableFrom(typeConverterType))
                    typeConverter = (TypeConverter)Activator.CreateInstance(typeConverterType);
            }

            if (typeConverter == null)
            {
                typeConverter = (isCollectionArgument)
                    ? new CollectionConverter(parameterType)
                    : TypeDescriptor.GetConverter(parameterType);
            }

            if (isCollectionArgument)
            {
                if (!typeConverter.CanConvertFrom(typeof(IEnumerable<string>)))
                    throw new InvalidOperationException();
            }
            else
            {
                if (!typeConverter.CanConvertFrom(typeof(string)))
                    throw new InvalidOperationException();
            }

            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)parameterInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault();

            _argumentName = (argumentAttribute != null)
                ? argumentAttribute.Name
                : string.Empty;
            _parameterName = parameterInfo.Name;
            _description = (descriptionAttribute != null)
                ? descriptionAttribute.Description
                : null;
            _position = parameterInfo.Position;

            SetFlag(
                ref _flags,
                Flags.IsRequired,
                !parameterInfo.IsOptional
                );
            SetFlag(
                ref _flags,
                Flags.AreMultipleAllowed,
                isCollectionArgument || parameterType.IsEnum && parameterType.IsDefined(typeof(FlagsAttribute), false)
                );

            _type = parameterType;
            _typeConverter = typeConverter;
            _defaultValue = parameterInfo.DefaultValue;
        }

        #endregion

        #region Properties

        internal string ArgumentName
        {
            get { return _argumentName; }
        }

        internal string ParameterName
        {
            get { return _parameterName; }
        }

        internal string DisplayName
        {
            get { return (_argumentName.Length == 0) ? _parameterName : _argumentName; }
        }

        internal string Description
        {
            get { return _description; }
        }

        internal int Position
        {
            get { return _position; }
        }

        internal bool IsRequired
        {
            get { return GetFlag(Flags.IsRequired); }
        }

        internal bool AreMultipleAllowed
        {
            get { return GetFlag(Flags.AreMultipleAllowed); }
        }

        internal bool IsInput
        {
            get { return GetFlag(Flags.IsInput); }
        }

        internal bool IsOutput
        {
            get { return GetFlag(Flags.IsOutput); }
        }

        internal bool IsError
        {
            get { return GetFlag(Flags.IsError); }
        }

        internal Type Type
        {
            get { return _type; }
        }

        internal TypeConverter TypeConverter
        {
            get { return _typeConverter; }
        }

        internal object DefaultValue
        {
            get { return _defaultValue; }
        }

        #endregion

        #region Private Methods

        private bool GetFlag(Flags requiredFlags)
        {
            return (_flags & requiredFlags) == requiredFlags;
        }

        private static void SetFlag(ref Flags flags, Flags requiredFlags, bool value)
        {
            if (value)
                flags |= requiredFlags;
            else
                flags &= ~requiredFlags;
        }

        #endregion
    }
}
