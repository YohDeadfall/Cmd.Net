using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

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
            IsError = IsOutput << 1,
            IsEnum = IsError << 1,
            IsBitField = IsEnum << 1 | IsEnum
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

            SetFlag(ref _flags, Flags.IsInput, parameterInfo.IsDefined(typeof(InputAttribute), false));
            SetFlag(ref _flags, Flags.IsOutput, parameterInfo.IsDefined(typeof(OutputAttribute), false));
            SetFlag(ref _flags, Flags.IsError, parameterInfo.IsDefined(typeof(ErrorAttribute), false));

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

            ArgumentAttribute argumentAttribute = parameterInfo.GetCustomAttribute<ArgumentAttribute>();

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
            }
            else
            {
                _argumentName = (argumentAttribute != null)
                    ? argumentAttribute.Name
                    : string.Empty;
                _parameterName = parameterInfo.Name;

                DescriptionAttribute descriptionAttribute = (DescriptionAttribute)parameterInfo.GetCustomAttribute<DescriptionAttribute>();

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
                    parameterType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(parameterType)
                    );
                SetFlag(
                    ref _flags,
                    Flags.IsEnum,
                    parameterType.IsEnum
                    );

                _type = parameterType;
                _defaultValue = parameterInfo.DefaultValue;

                SetTypeConverter(parameterInfo, AreMultipleAllowed, ref _typeConverter, ref _flags);
            }
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

        internal bool IsNamed
        {
            get { return _argumentName.Length != 0; }
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

        internal bool IsEnum
        {
            get { return GetFlag(Flags.IsEnum); }
        }

        internal bool IsBitField
        {
            get { return GetFlag(Flags.IsBitField); }
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

        private static void SetTypeConverter(ParameterInfo parameterInfo, bool allowMultipleValues, ref TypeConverter typeConverter, ref Flags flags)
        {
            TypeConverterAttribute typeConverterAttribute = parameterInfo.GetCustomAttribute<TypeConverterAttribute>();

            if (typeConverterAttribute != null && !string.IsNullOrEmpty(typeConverterAttribute.ConverterTypeName))
            {
                Type typeConverterType = Type.GetType(typeConverterAttribute.ConverterTypeName);

                if (typeConverterType != null && typeof(TypeConverter).IsAssignableFrom(typeConverterType))
                    typeConverter = (TypeConverter)Activator.CreateInstance(typeConverterType);
            }

            if (typeConverter == null)
            {
                Type parameterType = parameterInfo.ParameterType;

                if (parameterType.IsEnum)
                {
                    if (parameterType.IsDefined(typeof(FlagsAttribute), false))
                    {
                        typeConverter = new FlagEnumConverter(parameterType);
                        flags |= Flags.IsBitField;
                    }
                    else
                    {
                        typeConverter = new EnumConverter(parameterType);
                        flags |= Flags.IsEnum;
                    }
                }
                else
                {
                    if (allowMultipleValues)
                        typeConverter = new CollectionConverter(parameterType);
                    else
                        typeConverter = TypeDescriptor.GetConverter(parameterType);
                }
            }

            if (allowMultipleValues)
            {
                if (!typeConverter.CanConvertFrom(typeof(IEnumerable<string>)))
                    throw new InvalidOperationException();
            }
            else
            {
                if (!typeConverter.CanConvertFrom(typeof(string)))
                    throw new InvalidOperationException();
            }
        }

        #endregion
    }
}
