using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using DbCommon;
using EntityData;
using EntityManager.Common;
using EntityManager.View;

namespace EntityManager.ViewModels
{
    public class DatabaseTreeVM : ViewModelBase
    {
        private static DatabaseTreeVM self;

        private List<DatabaseElementTreeVm> root;
        private BObject selected;
        private ICommand selectedCommand;
        private ICommand changeDisplayLevelCommand;
        private int displayLevel = -1;  //display all levels by default

        //the root of the visual tree
        public List<DatabaseElementTreeVm> Root
        {
            get
            {
                //if (root != null) return root;

                //root = new List<DatabaseElementTreeVm>();

                //CDataProject dp;
                //using (var reader = new StreamReader(@"c:\Projects\Hunter\EntityDatabase\T4\Entity.xml"))
                //{
                //    var x = new XmlSerializer(typeof(CDataProject));
                //    dp = (CDataProject)x.Deserialize(reader);
                //}


                //var database = dp.DataBaseList[0];
                //FieldView._TableCollection = new ObservableCollection<Table>(database.TableList);

                //using (var writer = new StreamWriter(@"c:\1\test.xml"))
                //{
                //    var s = new XmlSerializer(dp.GetType());
                //    s.Serialize(writer, dp);
                //}


                //var databaseVm = new DatabaseElementTreeVm(database);
                //root.Add( databaseVm);
                return root;
            }
            set
            {
                root = value; 
                OnPropertyChanged("Root");
            }
        }

        public BObject Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                //selected.IsSelected = true;
                //ShowChildrenLevel();  //show only the levels chosen by the user
                OnPropertyChanged("Selected");
            }
        }

        public ICommand SelectedCommand
        {
            get
            {
                if (selectedCommand == null)
                {
                    selectedCommand = new CommandBase(i => this.SetSelected(i), null);
                }
                return selectedCommand;
            }
        }

        public ICommand ChangeDisplayLevelCommand
        {
            get
            {
                if (changeDisplayLevelCommand == null)
                {
                    changeDisplayLevelCommand = new CommandBase(i => ChangeDisplayLevel(i), null);
                }
                return changeDisplayLevelCommand;
            }
        }

        private void SetSelected(object orgElement)
        {
            //this.Selected = orgElement as DatabaseElementTreeVm;
            
        }

        private void ChangeDisplayLevel(object i)
        {
            int level;
            if (int.TryParse(i.ToString(), out level))
            {
                this.displayLevel = level;
                ShowChildrenLevel(); //show only the levels chosen by the user
            }
        }

        private void ShowChildrenLevel()
        {
            if (this.Selected != null)
            {
                //this.Selected.ShowChildrenLevel(this.displayLevel);
            }
        }

        private DatabaseTreeVM() { }

        public static DatabaseTreeVM Instance()
        {
            return self ?? (self = new DatabaseTreeVM());
        }
    }
}
