using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using EntityData;
using EntityManager.View;
using EntityManager.ViewModels;
using Microsoft.Win32;

namespace EntityManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = DatabaseTreeVM.Instance();

            RecentFileList.MenuClick += (s, e) => FileOpenCore(e.Filepath);
        }

        private string _filepath;
        CDataProject dp;

        private bool FileOpenCore(string filepath)
        {
            try
            {
                using (var reader = new StreamReader(filepath))
                {
                    var x = new XmlSerializer(typeof(CDataProject));
                    dp = (CDataProject)x.Deserialize(reader);
                }

                var database = dp.DataBaseList[0];
                RepairReferences(database);
                FieldView._TableCollection = new ObservableCollection<Table>(database.TableList);
                var databaseVm = new DatabaseElementTreeVm(database);
                var root = new List<DatabaseElementTreeVm>();
                root.Add(databaseVm);
                DatabaseTreeVM.Instance().Root = root;
                _filepath = filepath;

                return true;
            }
            catch
            {
                if (MessageBoxResult.Yes == MessageBox.Show("Do you want to remove this file from the recent file list?", "Failed to open file", MessageBoxButton.YesNo, MessageBoxImage.Question))
                    RecentFileList.RemoveFile(filepath);
                return false;
            }

        }

        private void RepairReferences(Database database)
        {
            CDatabaseEntity db = new CDatabaseEntity();
            database._table = new CTableDatabase(db);
            var tableTable = new CTableTable(db);
            var fieldTable = new CTableField(db);
            var relationTable = new CTableRelation(db);
            foreach (var t in database.TableList)
            {
                t.Database = database;
                t._table = tableTable;
                foreach (var f in t.FieldList)
                {
                    f._table = fieldTable;
                    f.Table = t;
                }
                foreach (var r in t.RelationList)
                {
                    r._table = relationTable;
                    r.Table = t;
                }
            }
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var el = (DatabaseElementTreeVm)e.NewValue;
            ((DatabaseTreeVM)DataContext).Selected = el.SubformObject;
            if (DockPanel1.Children.Count == 1)
            {
                DockPanel1.Children.RemoveAt(0);
            }
            DockPanel1.Children.Add((UIElement)Activator.CreateInstance(el.SubformType));
        }


        private void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Xml files (*.xml)|*.xml";
            openFileDialog1.RestoreDirectory = true;
            var showDialog = openFileDialog1.ShowDialog();

            if (showDialog != null && showDialog.Value)
            {
                if (FileOpenCore(openFileDialog1.FileName))
                    RecentFileList.InsertFile(openFileDialog1.FileName);
            }
        }

        private void CreateFile_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            using (var writer = new StreamWriter(_filepath))
            {
                var s = new XmlSerializer(dp.GetType());
                s.Serialize(writer, dp);
            }
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            //System.Windows.Application.Current.MainWindow.Close();
            Close();
        }

        private void SaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();

            openFileDialog1.Filter = "Xml files (*.xml)|*.xml";
            openFileDialog1.RestoreDirectory = true;
            var showDialog = openFileDialog1.ShowDialog();

            if (showDialog != null && showDialog.Value)
            {
                using (var writer = new StreamWriter(openFileDialog1.FileName))
                {
                    var s = new XmlSerializer(dp.GetType());
                    s.Serialize(writer, dp);
                }
                RecentFileList.InsertFile(openFileDialog1.FileName);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                using (var writer = new StreamWriter(_filepath))
                {
                    var s = new XmlSerializer(dp.GetType());
                    s.Serialize(writer, dp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла :" + ex.Message,"Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                e.Cancel = true;
            }

        }
    }
}
