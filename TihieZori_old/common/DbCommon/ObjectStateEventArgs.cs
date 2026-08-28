namespace DbCommon
{
    public class ObjectStateEventArgs
    {
        public ObjectStateEventArgs(BObject.ObjectStateType state)
        {
            NewState = state;
        }

        public BObject.ObjectStateType NewState
        {
            get;
            private set;
        }
 
    }
}