using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Cmd.Net
{
    internal sealed class FlagEnumConverter : EnumConverter
    {
        #region Fields

        private readonly KeyValuePair<char, ulong>[] _flagNamesAndValues;

        #endregion

        #region Constructors

        internal FlagEnumConverter(Type type)
            : base(type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Arg_MustBeEnum", "type");

            if (!Attribute.IsDefined(type, typeof(FlagsAttribute)))
                throw new ArgumentException("Arg_MustBeBitField", "type");

            _flagNamesAndValues = type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => new KeyValuePair<FlagAttribute, FieldInfo>(f.GetCustomAttribute<FlagAttribute>(), f))
                .Where(p => p.Key != null)
                .Select(p => new KeyValuePair<char, ulong>(char.ToUpperInvariant(p.Key.Name), ToUInt64(p.Value.GetValue(null))))
                .ToArray();
        }

        #endregion

        #region TypeConverter Members

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;

            if (str == null)
                return base.ConvertFrom(context, culture, value);

            ulong enumValue = 0;

            foreach (char ch in str)
                enumValue |= GetEnumValue(ch);

            return Enum.ToObject(EnumType, enumValue);
        }

        #endregion

        #region Private Methods

        private static ulong ToUInt64(object value)
        {
            TypeCode typeCode = Convert.GetTypeCode(value);
            ulong result;

            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    result = (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    break;

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Boolean:
                case TypeCode.Char:
                    result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return result;
        }

        private ulong GetEnumValue(char flagName)
        {
            flagName = char.ToUpperInvariant(flagName);
            CommandHelpers.ValidateFlagName("flagName", flagName);

            for (int i = 0; i < _flagNamesAndValues.Length; i++)
                if (_flagNamesAndValues[i].Key == flagName)
                    return _flagNamesAndValues[i].Value;

            throw new ArgumentException("flagName");
        }

        #endregion
    }
}
