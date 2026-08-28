using System;
using System.Collections.ObjectModel;
using DbCommon;
using EntityData;
using EntityManager.Common;

namespace EntityManager.ViewModels
{
    public class DatabaseVm : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        internal DatabaseVm(Database i)
        {
            this.Name = i.Name;
        }
    }
}
