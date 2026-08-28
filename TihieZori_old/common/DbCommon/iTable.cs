using System;
using System.Collections.Generic;
using System.IO;

namespace DbCommon
{
    public interface iTable
    {
        IEnumerable<FieldDescription> GetPrimaryKeysList();
        FieldDescription GetIdentity();
        IEnumerable<FieldDescription> GetLangPropertyList();
        string TableName();
        string TableName_select();
        CasheType GetCashType();
        CDatabase parentDataBase { get; set; }
        BObject CreateInstance();
        //B1Object<TPKey> GetById<TPKey>(TPKey id);
        Type GetRowType();

        iSortList CreateSortList();

        string OwnerName();

        ulong LastTS { get; set; }

        string GetFieldStringList(FieldDescription.fieldProp fieldProp);

        //void WriteAll(BinaryWriter bw);

        string GetFieldStringListU(FieldDescription.fieldProp fieldProp);

        void Save2(BObject bObject);

        iSortList ReadSortList(BinaryReader br);

        void ClearCash();

        bool IsChangesLog { get; set; }

        Type GetTableType();

        void Create();

        bool CreateTable(string DatabaseName);

        int GetCount(string filtr);

        string Name { get; set; }

        Dictionary<string, FieldDescription> Columns { get; set; }

        BObject GetById(Object id);

        bool Exists();

        ICustomFiltr GetCustomFiltr(string name, string[] @params);

        iSortList GetObjectList(CQueryDesc queryPager);

        iSortList GetObjectListByCustom(ICustomFiltr customFiltr, string OrderBy = null, string With = null, int RowCount = 0);
        void RestorePredefined();
        void Create2(bool startFilling = true);
    }
}