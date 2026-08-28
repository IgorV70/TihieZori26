using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbCommon;

namespace gpsDatabase.Postgree
{
    public static class Dialect
    {
        public static string CreateTable(this iTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE ").Append(table.Name).AppendLine("(");
            foreach (var column in table.Columns)
            {
                sb.Append("\t");
                column.Value.CreateDescription(sb);
            }
            if (table.GetPrimaryKeysList().Any())
            {
                sb.Append("\tCONSTRAINT " + table.Name + "_pkey PRIMARY KEY(")
                    .Append(table.PrimaryKeyList())
                    .AppendLine(")");
            }
            else
            {
                sb.Length -= 3;
                sb.AppendLine();
            }
            sb.AppendLine(");");
            return sb.ToString();
        }

        public static string PrimaryKeyList(this iTable table)
        {
            var ret = table.GetPrimaryKeysList().Aggregate("", (current, column) => current + (column.Name + ","));
            ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        public static StringBuilder CreateDescription(this FieldDescription column, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();
            sb.Append(column.Name).Append(" ").Append(column.SqlTypeName()).AppendLine(",");
            return sb;
        }
        public static string SqlTypeName(this FieldDescription column)
        {
            string typeName = column.PropInfo.PropertyType.Name;
            bool nullable = false;
            if (typeName == "Nullable`1")
            {
                typeName = Nullable.GetUnderlyingType(column.PropInfo.PropertyType).Name;
                nullable = true;
            }
            var ret = "";
            if ((column.Properties & FieldDescription.fieldProp.Identity) > 0)
                ret = "serial ";
            else
            {
                switch (typeName)
                {
                    case "DateTime":
                        ret = "timestamp";
                        break;
                    case "int":
                    case "Int32":
                        ret = "integer";
                        break;
                    case "string":
                    case "String":
                        ret = column.Size == 0
                            ? "character varying"
                            : string.Format("character varying({0})", column.Size);
                        break;
                    case "bool":
                    case "Boolean":
                        ret = "bit(1)";
                        break;
                    case "Image":
                    case "Byte[]":
                        ret = "bytea";
                        break;
                    default:
                        ret = typeName;
                        break;
                }
            }
            ret += nullable ? " NULL" : " NOT NULL";
            return ret;
        }
    }
}
