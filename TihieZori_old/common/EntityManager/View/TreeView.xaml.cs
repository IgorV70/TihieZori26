using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EntityManager.ViewModels;

namespace EntityManager.View
{
    /// <summary>
    /// Логика взаимодействия для TreeView.xaml
    /// </summary>
    public partial class TreeView : UserControl
    {
        public static readonly RoutedEvent SelectedChangedEvent;

        //public static DependencyProperty SelectedProperty;
        static TreeView()
        {
            //SelectedProperty = DependencyProperty.Register("Selected", typeof(DatabaseElementTreeVm), typeof(TreeView),
            //       new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedChanged)));
            SelectedChangedEvent = EventManager.RegisterRoutedEvent("SelectedChanged", RoutingStrategy.Bubble,
                   typeof(RoutedEvent), typeof(TreeView));
        }

        //public DatabaseElementTreeVm Selected
        //{
        //    get { return (DatabaseElementTreeVm)GetValue(SelectedProperty); }
        //    set { SetValue(SelectedProperty, value); }
        //}

        public TreeView()
        {
            InitializeComponent();
        }

        private void TvMain_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //throw new NotImplementedException();
            //SelectedChangedEvent.
        }

       
    }
}
