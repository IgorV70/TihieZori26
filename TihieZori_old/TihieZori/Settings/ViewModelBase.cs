using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace TihieZori.Settings
{
    public abstract class ViewModelBase //: INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                //handler(this, new PropertyChangedEventArgs(propertyName));
                handler(this, null);
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName]string name = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                if (name != null) OnPropertyChanged(name);
                return true;
            }
            return false;
        }

    }
}