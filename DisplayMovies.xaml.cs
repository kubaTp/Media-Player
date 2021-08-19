using System.Windows;

namespace Media_Player
{
    /// <summary>
    /// Logika interakcji dla klasy DisplayMovies.xaml
    /// </summary>
    public partial class DisplayMovies : Window
    {
        public DisplayMovies()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SqlCon sqlCon = new SqlCon();
            lvDataBinding.ItemsSource = sqlCon.getFromTable("BazaFilmow.sqlite");
        }

        private void LvDataBinding_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Produkt element = (Produkt)lvDataBinding.SelectedItems[0];
            string nazwa = element.Name;
        }

        public class Produkt
        {
            public string Name { get; set; }
        }
    }
}
