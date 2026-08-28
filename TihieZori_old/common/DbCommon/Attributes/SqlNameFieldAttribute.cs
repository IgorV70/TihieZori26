using System;
using System.Reflection;

namespace DbCommon.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SqlCommentFieldAttribute : Attribute
    {
        public SqlCommentFieldAttribute(string value)
        {
            Value = value;
        }
        public string Value { get; set; }
    }

    public static class SqlNameFieldExtensionClass
    {
        public static string Name<TEnum>(this TEnum enumElement) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return Enum.GetName(typeof (TEnum), enumElement);
        }

        public static string Comment<TEnum>(this TEnum enumElement) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var tp = typeof(TEnum);
            var field = tp.GetField(tp.GetEnumName(enumElement));
            var attr = field.GetCustomAttribute(typeof(SqlCommentFieldAttribute)) as SqlCommentFieldAttribute;

            return attr == null ? "" : attr.Value;
        }
    }
}
