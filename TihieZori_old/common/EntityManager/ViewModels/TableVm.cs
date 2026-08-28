using System;
using System.Collections.ObjectModel;
using DbCommon;
using EntityData;
using EntityManager.Common;

namespace EntityManager.ViewModels
{
    public class TableVm : ViewModelBase
    {
        private string _name;


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        internal TableVm(Table i)
        {
            this.Name = i.Name;
        }


    }
}
