using System;
using System.Reflection;

namespace Cmd.Net
{
    internal static class CustomAttributeExtensions
    {
        #region Public Methods

        public static T GetCustomAttribute<T>(this MemberInfo element) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(element, typeof(T));
        }

        public static T GetCustomAttribute<T>(this ParameterInfo element) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(element, typeof(T));
        }

        #endregion
    }
}
