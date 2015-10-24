using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Cmd.Net
{
    internal sealed class CollectionConverter : TypeConverter
    {
        #region Fields

        private readonly Type _collectionType;
        private readonly Type _itemType;
        private readonly TypeConverter _itemTypeConverter;
        private readonly bool _collectionIsArray;

        #endregion

        #region Constructors

        internal CollectionConverter(Type collectionType)
            : this(collectionType, null)
        {
        }

        internal CollectionConverter(Type collectionType, Type itemType)
        {
            if (collectionType == null)
                throw new ArgumentNullException("collectionType");

            if (!typeof(IEnumerable).IsAssignableFrom(collectionType))
                throw new ArgumentException();

            if (itemType == null)
                itemType = GetItemType(collectionType);
            else
                CheckItemType(collectionType, itemType);

            _collectionType = collectionType;
            _itemType = itemType;
            _itemTypeConverter = TypeDescriptor.GetConverter(itemType);
            _collectionIsArray =
                collectionType.IsArray ||
                collectionType == typeof(IEnumerable) ||
                collectionType == typeof(IEnumerable<>).MakeGenericType(itemType);
        }

        #endregion

        #region TypeConverter Members

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            if (_collectionType.IsAssignableFrom(sourceType))
                return true;

            if (typeof(IEnumerable).IsAssignableFrom(sourceType))
                return _itemTypeConverter.CanConvertFrom(GetItemType(sourceType));

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            if (_collectionType.IsInstanceOfType(value))
                return value;

            IEnumerable sourceEnumerable = value as IEnumerable;

            if (sourceEnumerable == null)
                return base.ConvertFrom(context, culture, value);

            if (_collectionIsArray)
            {
                IList sourceCollection = sourceEnumerable as IList;
                IList destinationArray;

                if (sourceCollection == null)
                {
                    IList destinationList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(_itemType));

                    foreach (object item in sourceEnumerable)
                        destinationList.Add(_itemTypeConverter.ConvertFrom(item));

                    destinationArray = (IList)Activator.CreateInstance(_itemType.MakeArrayType(), destinationList.Count);
                    destinationList.CopyTo((Array)destinationArray, 0);
                }
                else
                {
                    destinationArray = (IList)Activator.CreateInstance(_itemType.MakeArrayType(), sourceCollection.Count);

                    for (int i = 0; i < sourceCollection.Count; i++)
                        destinationArray[i] = _itemTypeConverter.ConvertFrom(sourceCollection[i]);
                }

                return destinationArray;
            }
            else
            {
                IList destinationList = (IList)Activator.CreateInstance(_collectionType);

                foreach (object item in sourceEnumerable)
                    destinationList.Add(_itemTypeConverter.ConvertFrom(item));

                return destinationList;
            }
        }

        #endregion

        #region Private Methods

        private static void CheckItemType(Type collectionType, Type itemType)
        {
            if (collectionType.IsGenericType &&
                collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                collectionType.GetGenericArguments()[0] == itemType)
            {
                return;
            }

            foreach (Type iface in collectionType.GetInterfaces())
            {
                if (iface.IsGenericType &&
                    iface.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                    iface.GetGenericArguments()[0] == itemType)
                {
                    return;
                }
            }

            throw new ArgumentException("itemType");
        }

        private static Type GetItemType(Type collectionType)
        {
            if (collectionType.IsGenericType &&
                collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return collectionType.GetGenericArguments()[0];
            }

            foreach (Type iface in collectionType.GetInterfaces())
            {
                if (iface.IsGenericType &&
                    iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return iface.GetGenericArguments()[0];
                }
            }

            return typeof(object);
        }

        private static bool TryGetCount(IEnumerable enumerable, out int count)
        {
            ICollection collection = enumerable as ICollection;

            if (collection != null)
            {
                count = collection.Count;
                return true;
            }

            PropertyInfo propertyCount = enumerable
                    .GetType()
                    .GetProperty("Count", typeof(int));

            if (propertyCount != null)
            {
                try
                {
                    count = (int)propertyCount.GetValue(enumerable, null);
                    return true;
                }
                catch
                {
                }
            }


            count = 0;
            return false;
        }

        #endregion
    }
}
