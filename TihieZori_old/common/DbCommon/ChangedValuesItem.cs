namespace DbCommon
{
    public class ChangedValuesItem
    {
        public string TableName = string.Empty;
        public string ColumnName = string.Empty;
        public string ValueOld = string.Empty;
        public string ValueNew = string.Empty;
        public bool RecordNew = false;
        public bool RecordDelete = false;
    }
}