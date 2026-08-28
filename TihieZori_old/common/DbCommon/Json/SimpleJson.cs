using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DbCommon.Json
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class JSonAttribute : Attribute
    {
        public virtual string ToJsonString(Object obj)
        { return ""; }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class JSonDecimal6 : JSonAttribute
    {
        private static readonly NumberFormatInfo Provider = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 6
        };
        public override string ToJsonString(object obj)
        {

            decimal value = (decimal)obj;
            return value.ToString("N", Provider);

        }
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class JSonDecimal3 : JSonAttribute
    {
        static readonly NumberFormatInfo Provider = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 3
        };

        public override string ToJsonString(object obj)
        {
            decimal value = (decimal)obj;
            return value.ToString("N", Provider);

        }
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class JSonDecimal1 : JSonAttribute
    {
        private static readonly NumberFormatInfo Provider = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 1
        };

        public override string ToJsonString(object obj)
        {
            decimal value = (decimal)obj;
            return value.ToString("N", Provider);

        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class JSonDecimal0 : JSonAttribute
    {
        static readonly NumberFormatInfo Provider = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
            NumberDecimalDigits = 0
        };

        public override string ToJsonString(object obj)
        {
            decimal value = (decimal)obj;
            return value.ToString("N", Provider);

        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class JSonPosix : JSonAttribute
    {
        public override string ToJsonString(object obj)
        {
            DateTime value = (DateTime)obj;
            int posix = (int)(value - (new DateTime(1970, 1, 1))).TotalSeconds;
            return posix.ToString("D");
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class JSonIp : JSonAttribute
    {
        public override string ToJsonString(object obj)
        {
            UInt32 val = (UInt32)obj;
            return "\"" + (val & 0xFF) + "." + ((val >> 8) & 0xFF) + "." + ((val >> 16) & 0xFF) + "." + (val >> 24) + "\"";
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class JSonMac : JSonAttribute
    {
        public override string ToJsonString(object obj)
        {
            string ret = "";
            UInt64 val = (UInt64)obj;
            for (int i = 0; i < 6; i++)
            {
                ret += ":" + (val & 0xFF).ToString("X2");
                val = val >> 8;
            }
            return "\"" + ret.Substring(1) + "\"";
        }
    }


    public class JsonObj
    { }

    public class JsonArray
    { }

    public static class SimpleJson
    {
        public static StringBuilder QuoteAppend(this StringBuilder sb, string s)
        {
            var len = s.Length;
            var escchar = new char[] { '\\', 'n', 'r', 't', '"' };
            for (var i = 0; i < len; i++)
            {
                var c = s[i];
                var iesc = "\\\n\r\t\"".IndexOf(c);
                if (iesc < 0)
                    sb.Append(c);
                else
                {
                    sb.Append('\\');
                    sb.Append(escchar[iesc]);
                }
            }
            return sb;
        }


        public static StringBuilder ToJson(StringBuilder sb, Object obj, JSonAttribute jsa=null)
        {
            sb = sb ?? new StringBuilder();

            if (obj == null)
                return sb.Append("null");

            var s = obj as string;
            if (s != null)
            {
                if (jsa != null)
                    return sb.Append(jsa.ToJsonString(s));
                sb.Append("\"");
                sb.QuoteAppend(s);
                return sb.Append("\"");
            }

            var os = obj as IEnumerable;
            if (os != null)
            {
                sb.Append("[");
                int len = sb.Length;
                foreach (object o in os)
                {
                    ToJson(sb, o, jsa);
                    sb.Append(",");
                }
                if (len < sb.Length) sb.Length--;
                sb.Append("]");
                return sb;
            }

            var array = obj as JsonArray;
            if (array != null)
            {
                sb.Append("[");
                FieldInfo[] fiArray = array.GetType().GetFields();
                foreach (var fi in fiArray)
                {
                    JSonAttribute jsonattr = fi.GetCustomAttributes(true).OfType<JSonAttribute>().FirstOrDefault();
                    ToJson(sb, fi.GetValue(array), jsonattr);
                    sb.Append(",");
                }
                if (sb[sb.Length - 1] == ',')
                    sb.Length--;
                sb.Append("]");
                return sb;
            }
            if (obj is JsonObj)
            {
                sb.Append("{");
                foreach (FieldInfo fi in obj.GetType().GetFields())
                {
                    JSonAttribute jsonattr = fi.GetCustomAttributes(true).OfType<JSonAttribute>().FirstOrDefault();
                    sb.AppendProperty(fi.Name, fi.GetValue(obj), jsonattr);
                    sb.Append(',');
                }
                if (sb[sb.Length - 1] == ',')
                    sb.Length--;
                sb.Append("}");
                return sb;
            }

            if (jsa != null)
            {
                sb.Append(jsa.ToJsonString(obj));
                return sb;
            }
            if (obj is int)
                return sb.Append((int)obj);
            if (obj is bool)
                return sb.Append((bool)obj ? "1" : "0");
            if (obj is DateTime)
                return sb.Append("\"" + ((DateTime)obj).ToString("u").Substring(0, 19) + "\"");

            return sb.Append(obj);

            //return retstring ? sb.ToString() : "";
        }

        public static void AppendProperty(this StringBuilder sb, string name, object value, JSonAttribute jsa = null)
        {
            sb.Append("\"");
            sb.Append(name);
            sb.Append("\":");
            ToJson(sb, value, jsa);
        }

    }
}
