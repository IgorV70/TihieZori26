using System.Collections.ObjectModel;
using System.Windows.Controls;
using EntityData;

namespace EntityManager.View
{
    /// <summary>
    /// Логика взаимодействия для DatabaseView.xaml
    /// </summary>
    public partial class FieldView : UserControl
    {
        public static ObservableCollection<Table> _TableCollection;

        public FieldView()
        {
            InitializeComponent();
        }

        public ObservableCollection<Table> TableCollection
        {
            get { return _TableCollection; }
        }
    
    }
}
