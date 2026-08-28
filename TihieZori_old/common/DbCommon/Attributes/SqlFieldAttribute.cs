using System;

namespace DbCommon.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SqlFieldAttribute : Attribute
    {
        public SqlFieldAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}