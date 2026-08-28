namespace DbCommon
{
    public interface IHasPrimaryKey
    {
    }

    public interface IPrimaryKey<TPKey> : IHasPrimaryKey
    {
        TPKey Id { get; set; }
    }
}