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

        private readonly string argumentName;
        private readonly string parameterName;
        private readonly string description;
        private readonly int position;
        private readonly Flags flags;
        private readonly Type type;
        private readonly TypeConverter typeConverter;
        private readonly object defaultValue;

        #endregion

        #region Constructors

        internal Argument(ParameterInfo parameterInfo)
        {
            Type parameterType = parameterInfo.ParameterType;

            SetFlag(ref this.flags, Flags.IsInput, parameterInfo.GetCustomAttributes(typeof(InputAttribute), true).Length > 0);
            SetFlag(ref this.flags, Flags.IsOutput, parameterInfo.GetCustomAttributes(typeof(OutputAttribute), true).Length > 0);
            SetFlag(ref this.flags, Flags.IsError, parameterInfo.GetCustomAttributes(typeof(ErrorAttribute), true).Length > 0);

            if (this.IsInput)
            {
                if (this.IsOutput || this.IsError)
                {
                    if (this.IsOutput == this.IsError)
                    { throw new InvalidOperationException(); }

                    if (!parameterType.IsAssignableFrom(typeof(Stream)))
                    { throw new InvalidOperationException(); }
                }
                else
                {
                    if (!parameterType.IsAssignableFrom(typeof(TextReader)))
                    { throw new InvalidOperationException(); }
                }
            }
            else
            {
                if (this.IsOutput || this.IsError)
                {
                    if (this.IsOutput == this.IsError)
                    { throw new InvalidOperationException(); }

                    if (!parameterType.IsAssignableFrom(typeof(TextWriter)))
                    { throw new InvalidOperationException(); }
                }
            }

            ArgumentAttribute argumentAttribute = (ArgumentAttribute)parameterInfo
                .GetCustomAttributes(typeof(ArgumentAttribute), true)
                .FirstOrDefault();

            if (this.IsInput || this.IsOutput || this.IsError)
            {
                if (argumentAttribute != null)
                { throw new InvalidOperationException(); }

                this.argumentName = parameterInfo.Name;
                this.parameterName = parameterInfo.Name;
                this.description = null;
                this.position = parameterInfo.Position;

                SetFlag(ref this.flags, Flags.IsRequired, true);
                SetFlag(ref this.flags, Flags.AreMultipleAllowed, false);

                this.type = parameterType;
                this.typeConverter = null;
                this.defaultValue = null;

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
                { typeConverter = (TypeConverter)Activator.CreateInstance(typeConverterType); }

                if (parameterType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(parameterType))
                {
                    if (!typeConverter.CanConvertFrom(typeof(IEnumerable<string>)))
                    { throw new InvalidOperationException(); }
                }
                else
                {
                    if (!typeConverter.CanConvertFrom(typeof(string)))
                    { throw new InvalidOperationException(); }
                }
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
                { throw new InvalidOperationException(); }
            }
            else
            {
                if (!typeConverter.CanConvertFrom(typeof(string)))
                { throw new InvalidOperationException(); }
            }

            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)parameterInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault();

            this.argumentName = (argumentAttribute != null)
                ? argumentAttribute.Name
                : string.Empty;
            this.parameterName = parameterInfo.Name;
            this.description = (descriptionAttribute != null)
                ? descriptionAttribute.Description
                : null;
            this.position = parameterInfo.Position;

            SetFlag(
                ref this.flags,
                Flags.IsRequired,
                !parameterInfo.IsOptional
                );
            SetFlag(
                ref this.flags,
                Flags.AreMultipleAllowed,
                isCollectionArgument || parameterType.IsEnum && parameterType.IsDefined(typeof(FlagsAttribute), false)
                );

            this.type = parameterType;
            this.typeConverter = typeConverter;
            this.defaultValue = parameterInfo.DefaultValue;
        }

        #endregion

        #region Properties

        internal string ArgumentName
        {
            get { return argumentName; }
        }

        internal string ParameterName
        {
            get { return parameterName; }
        }

        internal string DisplayName
        {
            get { return (argumentName.Length == 0) ? parameterName : argumentName; }
        }

        internal string Description
        {
            get { return description; }
        }

        internal int Position
        {
            get { return position; }
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
            get { return type; }
        }

        internal TypeConverter TypeConverter
        {
            get { return typeConverter; }
        }

        internal object DefaultValue
        {
            get { return defaultValue; }
        }

        #endregion

        #region Private Methods

        private bool GetFlag(Flags requiredFlags)
        {
            return (flags & requiredFlags) == requiredFlags;
        }

        private static void SetFlag(ref Flags flags, Flags requiredFlags, bool value)
        {
            if (value)
            { flags |= requiredFlags; }
            else
            { flags &= ~requiredFlags; }
        }

        #endregion
    }
}
