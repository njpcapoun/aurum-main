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

		/// <summary>
		/// Constructor for RoomSearchPage.
		/// </summary>
        public RoomSearchPage()
        {
            InitializeComponent();
            DataContext = Model;
        }

		/// <summary>
		/// Search for available room when search button is clicked.
		/// </summary>
		/// <param name="sender">A reference to the control/object that raised the event.</param>
		/// <param name="e">State information and event data associated with a routed event.</param>
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
