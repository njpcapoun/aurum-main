using ClassroomAssignment.ViewModel;
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

namespace ClassroomAssignment.UI
{
    /// <summary>
    /// Interaction logic for RoomSearchPage.xaml
    /// </summary>
    public partial class RoomSearchPage : Page
    {
        SearchViewModel Model = new SearchViewModel();

        public RoomSearchPage()
        {
            InitializeComponent();
            DataContext = Model;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if(SearchControl.ParametersValid)
            {
                var searchParameters = SearchControl.SearchParameters;
                Model.Search(searchParameters);
            }
            
        }
    }
}
