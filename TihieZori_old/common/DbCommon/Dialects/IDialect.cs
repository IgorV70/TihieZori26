namespace DbCommon.Dialects
{
    public interface IDialect
    {
        string CreateDatabase();

        string DeleteDatabase();
        string TestQuery();
        string CreateTable(iTable t);

        string ConvertToString(byte[] buffer);

        string ConvertToString(short[] p);

        string SelectLastId(FieldDescription identity);

        string DeleteCommand();

        string TableExists(iTable t);

        string ConvertToBooleanString(object value);
        string IdentityInsert<T, TDatabase, TPKey>(CTable<T, TDatabase, TPKey> cTable)
            where T : BObject, new()
            where TDatabase : CDatabase;
        string AddColumn(iTable t, FieldDescription fieldDescription);
        string TruncateTable(iTable cTable);
    }
}