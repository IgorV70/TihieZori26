using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DbCommon;
using EntityData;
using EntityManager.Common;
using EntityManager.View;

namespace EntityManager.ViewModels
{
    public class DatabaseElementTreeVm : ViewModelBase
    {
        private Guid _id;
        private string _name;
        private string _imagePath;
        private Type _subformType;
        private BObject _object;
        private ObservableCollection<DatabaseElementTreeVm> _children = new ObservableCollection<DatabaseElementTreeVm>();

        private bool _isSelected;
        private bool _isExpanded;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public DatabaseElementTreeVm Parent;

        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        public Type SubformType
        {
            get { return _subformType; }
            set { _subformType = value; }
        }
        public BObject SubformObject
        {
            get { return _object; }
            set { _object = value; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }
        public ObservableCollection<DatabaseElementTreeVm> Children
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
                OnPropertyChanged("Children");
            }
        }

        internal DatabaseElementTreeVm(Database i)
        {
            this.Id = i.Id;
            this.Name = i.Name;
            this.ImagePath = @"/Resources/database.png";
            this.SubformType = typeof(DatabaseView);
            this.SubformObject = i;
            this.IsExpanded = true;
            this.CntMenu = CreateDatabaseMenu(this);
            foreach (var t in i.TableList)
            {
                _children.Add(new DatabaseElementTreeVm(t, this));
            }
        }

        private ContextMenu CreateDatabaseMenu(DatabaseElementTreeVm databaseElementTreeVm)
        {
            ContextMenu ret = new ContextMenu();
            var addTable = new MenuItem
            {
                Header = "Добавить таблицу",
                DataContext = databaseElementTreeVm
            };
            addTable.Click += AddTableOnClick;
            ret.Items.Add(addTable);
            return ret;
        }

        private void AddTableOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItem mItem = (MenuItem)sender;
            var item = (DatabaseElementTreeVm)mItem.DataContext;
            var tbl = item.SubformObject as Table;
            int insertIndex = 0;
            Database db = null;
            if (tbl != null)
            {
                db = tbl.Database;
                insertIndex = db.TableList.IndexOf(tbl) + 1;
                item = item.Parent;
            }

            db = db ?? item.SubformObject as Database;
            if (db != null)
            {
                var name = "Новая таблица";
                int i = 0;
                while (db.TableList.Exists(t => t.Name == name))
                {
                    i++;
                    name = string.Format("Новая таблица({0})", i);
                }
                var table = new Table { Name = name, Database = db };
                var fieldId = new Field() { Name = "Id", Table = table, FType = "int", IsIdentity = true, IsPrimary = true };
                var fieldName = new Field() { Name = "Name", Table = table, FType = "nvarchar", Size = 256 };
                table.FieldList.Add(fieldId);
                table.FieldList.Add(fieldName);

                db.TableList.Insert(insertIndex, table);
                item.Children.Insert(insertIndex, new DatabaseElementTreeVm(table, item));
            }
        }

        internal DatabaseElementTreeVm(Table i, DatabaseElementTreeVm parent)
        {
            this.Id = i.Id;
            this.Name = i.Name;
            this.ImagePath = @"/Resources/table.png";
            this.SubformType = typeof(TableView);
            this.SubformObject = i;
            this.CntMenu = CreateTableMenu(this);
            this.Parent = parent;
            foreach (var f in i.FieldList)
            {
                _children.Add(new DatabaseElementTreeVm(f, this));
            }
            foreach (var r in i.RelationList)
            {
                _children.Add(new DatabaseElementTreeVm(r, this));
            }
        }

        private ContextMenu CreateTableMenu(DatabaseElementTreeVm databaseElementTreeVm)
        {
            ContextMenu ret = new ContextMenu();
            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem { Header = "Добавить таблицу" });
            items.Add(new MenuItem { Header = "Скопировать таблицу" });
            items.Add(new MenuItem { Header = "Удалить таблицу" });
            items.Add(new MenuItem { Header = "Добавить поле" });
            items.Add(new MenuItem { Header = "Добавить связь" });
            items[0].Click += AddTableOnClick;
            items[1].Click += AddTableCopyOnClick;
            items[2].Click += RemoveTableOnClick;
            items[3].Click += AddFieldOnClick;
            items[4].Click += AddRelationOnClick;
            foreach (var item in items)
            {
                item.DataContext = databaseElementTreeVm;
                ret.Items.Add(item);
            }
            return ret;
        }

        private void AddFieldOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItem mItem = (MenuItem)sender;
            var item = (DatabaseElementTreeVm)mItem.DataContext;
            var fld = item.SubformObject as Field;
            int insertIndex = 0;
            Table tbl = null;
            if (fld != null)
            {
                tbl = fld.Table;
                insertIndex = tbl.FieldList.IndexOf(fld) + 1;
                item = item.Parent;
            }

            tbl = tbl ?? item.SubformObject as Table;
            if (tbl != null)
            {
                var name = "Новое поле";
                int i = 0;
                while (tbl.FieldList.Exists(t => t.Name == name))
                {
                    i++;
                    name = string.Format("Новое поле({0})", i);
                }
                var field = new Field() { Name = name, Table = tbl, FType = "int" };

                tbl.FieldList.Insert(insertIndex, field);
                item.Children.Insert(insertIndex, new DatabaseElementTreeVm(field, item));
            }
        }

        private void AddRelationOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItem mItem = (MenuItem)sender;
            var item = (DatabaseElementTreeVm)mItem.DataContext;
            var fld = item.SubformObject as Field;
            Table tbl = null;
            if (fld != null)
            {
                tbl = fld.Table;
                item = item.Parent;
            }

            tbl = tbl ?? item.SubformObject as Table;
            if (tbl != null)
            {
                var name = "Связь";
                int i = 0;
                while (tbl.FieldList.Exists(t => t.Name == name))
                {
                    i++;
                    name = string.Format("Связь({0})", i);
                }
                var relation = new Relation()
                {
                    Name = name,
                    Table = tbl,
                    RType = "dependent",
                    PrimaryKeyTable = tbl.Name,
                    PrimaryKeyFieldList = "Id",
                    ForeignKeyFieldList = tbl.Name + "Id"
                };

                tbl.RelationList.Add(relation);
                item.Children.Add(new DatabaseElementTreeVm(relation, item));
            }
        }

        private void AddFieldCopyOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItem mItem = (MenuItem)sender;
            var item = (DatabaseElementTreeVm)mItem.DataContext;
            var fld = item.SubformObject as Field;
            if (fld != null)
            {
                var tbl = fld.Table;
                var insertIndex = tbl.FieldList.IndexOf(fld) + 1;

                var name = fld.Name;
                int i = 0;
                while (tbl.FieldList.Exists(t => t.Name == name))
                {
                    i++;
                    name = string.Format("{0}({1})", fld.Name, i);
                }
                var field = (Field)fld.Clone();
                field.Name = name;
                field.Table = tbl;
                tbl.FieldList.Insert(insertIndex, field);
                item.Parent.Children.Insert(insertIndex, new DatabaseElementTreeVm(field, item.Parent));
            }
        }

        private void AddTableCopyOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItem mItem = (MenuItem)sender;
            var item = (DatabaseElementTreeVm)mItem.DataContext;
            var tbl = item.SubformObject as Table;
            if (tbl != null)
            {
                var db = tbl.Database;
                var insertIndex = db.TableList.IndexOf(tbl) + 1;

                var name = tbl.Name;
                int i = 0;
                while (db.TableList.Exists(t => t.Name == name))
                {
                    i++;
                    name = string.Format("{0}({1})", tbl.Name, i);
                }
                var table = (Table)tbl.DeepClone();
                table.Id = Guid.NewGuid();
                table.Name = name;
                table.Database = db;
                foreach (var field in table.FieldList)
                {
                    field.Id = Guid.NewGuid();
                    field.Table = table;
                }
                db.TableList.Insert(insertIndex, table);
                item.Parent.Children.Insert(insertIndex, new DatabaseElementTreeVm(table, item.Parent));
            }
        }

        private void RemoveTableOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItem mItem = (MenuItem)sender;
            var item = (DatabaseElementTreeVm)mItem.DataContext;
            var tbl = item.SubformObject as Table;
            if (tbl != null)
            {
                if (
                    MessageBox.Show(string.Format("Удалить таблицу {0} ?", tbl.Name), "Удаление таблицы",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    tbl.Database.TableList.Remove(tbl);
                    item.Parent.Children.Remove(item);
                }
            }
        }

        private void RemoveFieldOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItem mItem = (MenuItem)sender;
            var item = (DatabaseElementTreeVm)mItem.DataContext;
            var fld = item.SubformObject as Field;
            if (fld != null)
            {
                if (
                    MessageBox.Show(string.Format("Удалить поле {0} ?", fld.Name), "Удаление поля",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    fld.Table.FieldList.Remove(fld);
                    item.Parent.Children.Remove(item);
                }
            }
        }

        private void RemoveRelationOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            MenuItem mItem = (MenuItem)sender;
            var item = (DatabaseElementTreeVm)mItem.DataContext;
            var rel = item.SubformObject as Relation;
            var table = item.Parent.SubformObject as Table;
            if (rel != null && table != null)
            {
                if (
                    MessageBox.Show(string.Format("Удалить связь {0} ?", rel.Name), "Удаление связи",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    table.RelationList.Remove(rel);
                    item.Parent.Children.Remove(item);
                }
            }
        }

        internal DatabaseElementTreeVm(Field i, DatabaseElementTreeVm parent)
        {
            this.Id = i.Id;
            this.Name = i.Name;
            this.ImagePath = @"/Resources/field.png";
            this.SubformType = typeof(FieldView);
            this.SubformObject = i;
            this.CntMenu = CreateFieldMenu(this);
            this.Parent = parent;
        }

        internal DatabaseElementTreeVm(Relation i, DatabaseElementTreeVm parent)
        {
            this.Id = i.Id;
            this.Name = i.Name;
            this.ImagePath = @"/Resources/relation.png";
            this.SubformType = typeof(RelationView);
            this.SubformObject = i;
            this.CntMenu = CreateRelationMenu(this);
            this.Parent = parent;
        }

        private ContextMenu CreateRelationMenu(DatabaseElementTreeVm databaseElementTreeVm)
        {
            ContextMenu ret = new ContextMenu();
            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem { Header = "Добавить связь" });
            items.Add(new MenuItem { Header = "Удалить связь" });
            //items.Add(new MenuItem { Header = "Добавить поле" });
            items[0].Click += AddRelationOnClick;
            items[1].Click += RemoveRelationOnClick;
            foreach (var item in items)
            {
                item.DataContext = databaseElementTreeVm;
                ret.Items.Add(item);
            }
            return ret;
        }

        private ContextMenu CreateFieldMenu(DatabaseElementTreeVm databaseElementTreeVm)
        {
            ContextMenu ret = new ContextMenu();
            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem { Header = "Добавить поле" });
            items.Add(new MenuItem { Header = "Добавить копию" });
            items.Add(new MenuItem { Header = "Удалить поле" });
            items.Add(new MenuItem { Header = "Добавить связь" });
            //items.Add(new MenuItem { Header = "Добавить поле" });
            items[0].Click += AddFieldOnClick;
            items[1].Click += AddFieldCopyOnClick;
            items[2].Click += RemoveFieldOnClick;
            items[3].Click += AddRelationOnClick;
            //items[2].Click += AddFieldOnClick;
            foreach (var item in items)
            {
                item.DataContext = databaseElementTreeVm;
                ret.Items.Add(item);
            }
            return ret;

        }

        public ContextMenu CntMenu { get; set; }

        internal void ShowChildrenLevel(int levelsShown)
        {
            //if (levelsShown == -1) //show all levels
            //    this.Children = GetChildren();
            //else if (levelsShown == 0)  //don't show any more levels
            //    this.Children = new ObservableCollection<DatabaseElementTreeVm>();  //set as empty
            //else if (levelsShown > 0)  //if a level is requested
            //{
            //    this.Children = GetChildren();
            //    foreach (DatabaseElementTreeVm i in this.Children)
            //        i.ShowChildrenLevel(levelsShown - 1);  //decrement 1 for next level
            //}
        }

    }
}
