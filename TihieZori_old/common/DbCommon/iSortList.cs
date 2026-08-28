using System;
using System.Collections;
using System.IO;

namespace DbCommon
{
    public interface iSortList : IEnumerable
    {
        iTable Table { get; }
        void Add(BObject obj);
        BObject Find(Predicate<BObject> pr);
        iSortList FindAll(Predicate<BObject> pr);
        void WriteAll(BinaryWriter bw);
        //void ChangeLang(string mnem);
        int GetCount();
        string TableName();
        void Remove(BObject obj);
        void SetSortComparer(string Expression);
        void Save();
    }
}