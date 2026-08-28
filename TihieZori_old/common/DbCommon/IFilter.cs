namespace DbCommon
{
    public interface IFilter<T>
    {
        bool Test(T obj);

    }
}