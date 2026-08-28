using System;
using System.Linq;
using System.Text;

namespace DbCommon.Dialects
{
    public class DialectPostGree : IDialect
    {

        public string CreateDatabase()
        {
            return @"CREATE DATABASE ""{0}""
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'Russian_Russia.1251'
       LC_CTYPE = 'Russian_Russia.1251'
       CONNECTION LIMIT = -1;";
        }

        public string DeleteDatabase()
        {
            return @"DROP DATABASE ""{0}""";
        }

        public string TestQuery() {
            return @"SET statement_timeout = 0;
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
set client_encoding='1251';
SET search_path = _0008355b, pg_catalog;";
        }

        public string TableExists(iTable t)
        {
            throw new NotImplementedException();
        }

        public string CreateTable(iTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE ").Append(table.Name).AppendLine("(");
            foreach (var column in table.Columns)
            {
                sb.Append("\t");
                sb.Append(CreateColumnDescription(column.Value));
                sb.AppendLine(",");
            }
            if (table.GetPrimaryKeysList().Any())
            {
                sb.Append("\tCONSTRAINT " + table.Name + "_pkey PRIMARY KEY(")
                    .Append(PrimaryKeyList(table))
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

        public string TruncateTable(iTable table)
        {
            return $"TRUNCATE TABLE ONLY {table.Name} RESTART IDENTITY";
        }


        private string PrimaryKeyList(iTable table)
        {
            var ret = table.GetPrimaryKeysList().Aggregate("", (current, column) => current + (column.Name + ","));
            ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        private string CreateColumnDescription(FieldDescription column)
        {
            return column.Name + " " + SqlTypeName(column);
        }

        public string SqlTypeName(FieldDescription column)
        {
            Type columnType = column.PropInfo.PropertyType;
            Type columnBaseType = columnType.BaseType;
            if (columnBaseType == typeof(Enum))
                return "int";
            string typeName = columnType.Name;
            bool nullable = (column.Properties & FieldDescription.fieldProp.Nullable) > 0;
            if (typeName == "Nullable`1")
            {
                // ReSharper disable once PossibleNullReferenceException
                typeName = Nullable.GetUnderlyingType(columnType).Name;
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

        public string ConvertToString(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder(buffer.Length * 2 + 6);
            sb.Append(@"E'\\x");
            foreach (byte b in buffer)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            sb.Append(@"'");
            return sb.ToString();
        }


        public string ConvertToString(short[] buffer)
        {
            StringBuilder sb = new StringBuilder(buffer.Length * 4 + 6);
            sb.Append(@"E'\\x");
            foreach (short b in buffer)
                sb.Append(Convert.ToString(b, 16).PadLeft(4, '0'));
            sb.Append(@"'");
            return sb.ToString();
        }
        public string SelectLastId(FieldDescription identity)
        {
            return "select cast(lastval() as " + identity.sType + ")";
        }


        public string DeleteCommand()
        {
            return "delete from {1} where {2}";
        }


        public string ConvertToBooleanString(object value)
        {
            if (value is bool)
                return (bool) value ? "b'1'" : "b'0'";
            int intVal = Convert.ToInt32(value);
            return intVal != 0 ? "b'1'" : "b'0'";
        }

        public string IdentityInsert<T, TDatabase, TPKey>(CTable<T, TDatabase, TPKey> cTable)
            where T : BObject, new()
            where TDatabase : CDatabase
        {
            return ";";
        }

        private string ColumnTypeName(FieldDescription column)
        {
            Type columnType = column.PropInfo.PropertyType;
            Type columnBaseType = columnType.BaseType;
            if (columnBaseType == typeof(Enum))
                return "int";
            string typeName = columnType.Name;
            var ret = "";
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
            return ret;
        }
        public string AddColumn(iTable t, FieldDescription fd)
        {
            return $"alter table {t.OwnerName()}.{t.TableName()} add {fd.Name} {ColumnTypeName(fd)}";
        }

    }
}