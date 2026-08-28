using System.Text;
using DbCommon.Dialects;

namespace DbCommon
{
    public class CQueryDesc
    {
        public CQueryDesc(iTable t)
                        : this(t, (string)null, null, null, 0)
        {
        }

        public CQueryDesc(iTable t, string filtr)
                        : this(t, filtr, null, null, 0)
        {
        }

        public CQueryDesc(iTable t, ICustomFiltr filtr)
                        : this(t, filtr, null, null, 0)
        {
        }

        public CQueryDesc(iTable t, string filtr, int rowcount)
            : this(t, filtr, null, null, rowcount)
        {
        }

        public CQueryDesc(iTable t, ICustomFiltr filtr, int rowcount)
            : this(t, filtr, null, null, rowcount)
        {
        }
        public CQueryDesc(iTable t, string filtr, string order)
            : this(t, filtr, order, null, 0)
        {
        }

        public CQueryDesc(iTable t, ICustomFiltr filtr, string order)
    : this(t, filtr, order, null, 0)
        {
        }

        public CQueryDesc(iTable t, string filtr, string order, int rowcount)
            : this(t, filtr, order, null, rowcount)
        {
        }

        public CQueryDesc(iTable t, ICustomFiltr filtr, string order, int rowcount)
             : this(t, filtr, order, null, rowcount)
        {
        }

        public CQueryDesc(iTable t, string filtr, string order, string with, int rowcount)
            : this(t, (ICustomFiltr)null, order, with, rowcount)
        {
            this.Filtr = filtr;
            if (this.Filtr == "1" || this.Filtr == "b'1'")
                this.Filtr = null;
        }

        public CQueryDesc(iTable t, ICustomFiltr customFiltr, string order, string with, int rowcount)
        {
            this.T = t;
            this.CustomFiltr = customFiltr;
            this.Order = order;
            this.With = with;
            this.Rowcount = rowcount;
            _databaseType = t.parentDataBase.DatabaseType;
            _dialect = t.parentDataBase.Dialect;
        }

        public iTable T;
        public string Filtr;
        public ICustomFiltr CustomFiltr;
        public string Order;
        public string With;
        public int Rowcount;
        protected string Fields;
        private IDialect _dialect = null;
        private DatabaseType _databaseType;

        public StringBuilder QueryWhere(StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(Filtr))
            {
                sb.AppendLine();
                sb.Append(" where "); sb.Append(Filtr); sb.Append(" ");
                return sb;
            }
            if (CustomFiltr != null)
            {
                sb.AppendLine();
                sb.Append(" where "); sb.Append(CustomFiltr.WhereTerm(_databaseType,"t")); sb.Append(" ");
                return sb;

            }

            return sb;
        }

        public virtual StringBuilder QuerySelect(StringBuilder sb)
        {
            if (sb == null) sb = new StringBuilder(512);

            sb.Append("select ");
            if (Rowcount > 0 && _databaseType == DatabaseType.MsSql)
            {
                sb.Append("top "); sb.Append(Rowcount); sb.Append(" ");
            }
            sb.Append(T.GetFieldStringList(FieldDescription.fieldProp.All));
            sb.Length--;
            sb.Append(" from ");
            if (_databaseType == DatabaseType.MsSql)
            {
                sb.Append(T.OwnerName());
                sb.Append(".");
            }
            sb.Append(T.TableName_select());
            sb.Append(" t ");
            if (!string.IsNullOrEmpty(With))
            {
                sb.Append(" with("); sb.Append(With); sb.Append(")");
            }
            QueryWhere(sb);

            if (!string.IsNullOrEmpty(Order))
            {
                sb.AppendLine();
                sb.Append(" order by "); sb.Append(Order); sb.Append(" ");
            }
            if (_databaseType == DatabaseType.PostgreeSql)
            {
                if (Rowcount > 0)
                {
                    sb.AppendLine();
                    sb.Append("Limit ");
                    sb.Append(Rowcount);
                }
                sb.Append(";");
            }

            return sb;
        }
    }

}