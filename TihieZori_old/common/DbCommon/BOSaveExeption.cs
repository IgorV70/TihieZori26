namespace DbCommon
{
    /// <summary>
    /// Это исключение следует вызывать при проверке данных а методе Save
    /// и оно же перехватывается методом SaveTry
    /// </summary>
    public class BOSaveExeption : System.Exception
    {
        public BOSaveExeption(string msg)
            : base(msg)
        { }


        internal void Show()
        {
            BObject.ShowErrorMessage(this.Message);
        }
    }

}