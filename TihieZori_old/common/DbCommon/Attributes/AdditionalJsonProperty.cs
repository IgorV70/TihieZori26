using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommon.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AdditionalJsonPropertyAttribute : Attribute
    {
        string _propertyName;
        public AdditionalJsonPropertyAttribute(string propertyName)
        {
            _propertyName = propertyName;
        }

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }
    }
}
