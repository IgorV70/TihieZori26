using System;
using System.Linq;
using System.Text;

namespace DbCommon.Dialects
{
    public class DialectMsSql : IDialect
    {

        public string CreateDatabase()
        {
            return "CREATE DATABASE [{0}]";
        }

        public string DeleteDatabase()
        {
            return "DROP DATABASE [{0}]";
        }

        public string TestQuery()
        {
            return "select 1;";
        }


        //select 1 from sysobjects where name='Device' and xtype='u'
        public string TableExists(iTable table)
        {
            return string.Format("select 1 from sysobjects where name='{0}' and xtype like '[uV]'", table.Name);
        }

        public string CreateTable(iTable table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE [{0}].[{1}](", table.OwnerName(), table.Name);

            foreach (var column in table.Columns)
            {
                sb.Append("\t");
                CreateColumnDescription(sb, column.Value);
                sb.AppendLine(",");
            }
            if (table.GetPrimaryKeysList().Any())
            {
                sb.Append("\tCONSTRAINT PK_" + table.Name + " PRIMARY KEY CLUSTERED(")
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
            return $"TRUNCATE TABLE {table.OwnerName()}.{table.Name}";
        }

        private string PrimaryKeyList(iTable table)
        {
            var ret = table.GetPrimaryKeysList().Aggregate("", (current, column) => current + (column.Name + ","));
            ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }


        private StringBuilder CreateColumnDescription(StringBuilder sb, FieldDescription column)
        {
            sb.Append(column.Name);
            sb.Append(" ");
            SqlTypeName(sb, column);
            return sb;
        }

        private StringBuilder SqlTypeName(StringBuilder sb, FieldDescription column)
        {
            sb.Append(column.SqlType);
            sb.Append(" ");
            if ((column.Properties & FieldDescription.fieldProp.Identity) > 0)
                sb.Append("IDENTITY(1,1) ");
            if (column.Size > 0 && column.Size2 > 0)
                sb.AppendFormat("({0},{1}) ", column.Size, column.Size2);
            else if (column.Size > 0)
                sb.AppendFormat("({0}) ", column.Size);
            sb.Append((column.Properties & FieldDescription.fieldProp.Nullable) > 0
                ? "NULL "
                : "NOT NULL ");
            return sb;
        }


        public string ConvertToString(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder(buffer.Length * 2 + 2);
            sb.Append("0x");
            foreach (byte b in buffer)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString();
        }


        public string ConvertToString(short[] buffer)
        {
            StringBuilder sb = new StringBuilder(buffer.Length * 4 + 2);
            sb.Append("0x");
            foreach (short b in buffer)
                sb.Append(Convert.ToString(b, 16).PadLeft(4, '0'));
            return sb.ToString();
        }


        public string SelectLastId(FieldDescription identity)
        {
            return "select cast(@@identity as " + identity.sType + ")";
        }


        public string DeleteCommand()
        {
            return "delete {0}.{1} where {2}";
        }


        public string ConvertToBooleanString(object value)
        {
            if (value is bool)
                return (bool)value ? "1" : "0";
            return Convert.ToInt32(value).ToString();
        }

        public string IdentityInsert<T, TDatabase, TPKey>(CTable<T, TDatabase, TPKey> cTable)
            where T : BObject, new()
            where TDatabase : CDatabase
        {
            var owner = cTable.OwnerName();
            var table = cTable.TableName();
            var opt = cTable.IdentityInsert ? "ON" : "OFF";
            return $"SET IDENTITY_INSERT {owner}.{table} {opt};";
        }

        private string ColumnTypeName(FieldDescription column)
        {
            var ret = column.SqlType;
            if (column.Size > 0 && column.Size2 > 0)
                ret += $"({column.Size},{column.Size2})";
            else if (column.Size > 0)
                ret += $"({column.Size})";
            return ret;
        }
        public string AddColumn(iTable t, FieldDescription fd)
        {
            return $"alter table {t.OwnerName()}.{t.TableName()} add {fd.Name} {ColumnTypeName(fd)}";
        }
    }
}