using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Media_Player
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;

        #region Constructor
        /// <summary>
        /// Glowny konstruktor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //stworzenie nowego obiektu timer o typie DispatcherTimer
            timer = new DispatcherTimer();
            //zakdeklarowanie wartości, który pozwalają określić interwały timera
            timer.Interval = TimeSpan.FromMilliseconds(500);
            //okresla przesunięcie Timera
            timer.Tick += Timer_Tick;

            //nawiązanie połączenia z bazą i zamnknięcie połączenia
            sqlCon.IfDatabaseExist(baza);
            sqlCon.CloseConnection();
        }

        public string baza = "BazaFilmow.sqlite";
        public SqlCon sqlCon = new SqlCon();

        private void Timer_Tick(object sender, EventArgs e)
        {
            slider_seek.Value = mediaElement1.Position.TotalSeconds;
        }
        #endregion

        #region funkcje
        /// <summary>
        /// zawiera wszystkie funkje itd.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        //Play button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //wbudowana funkcja umożliwiającą odtworzenie pliku upuszczonego na MediaElement o nazwie "mediaElemnt1"
            mediaElement1.Play();
        }

        //Pause button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //wbudowana funkcja umożliwiającą zatrzymanie odtawarzania pliku upuszczonego na MediaElement o nazwie "mediaElemnt1"
            mediaElement1.Pause();
        }

        //Stop button
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //wbudowana funkcja umożliwiającą przesunięcie pliku upuszczonego na MediaElement o nazwie "mediaElemnt1" do początku
            mediaElement1.Stop();
        }

        //History button
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //otworzenie nowego okna bez zamknięcia biężącego okna 
            var movies = new DisplayMovies();
            movies.Show();
        }

        //określenie głośności elemntu w zależności od suwaka głośności
        private void slider_vol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //głośność będzie zależna od położenia zasuwaka
            mediaElement1.Volume = (double)slider_vol.Value;
        }

        //określenie postępu w odtwarzaniu pliku
        private void slider_seek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //wartośc suwaka postępu jest zależna od postępu filmu
            mediaElement1.Position = TimeSpan.FromSeconds(slider_seek.Value);
        }

        //wydarzenie kiedy plik jest uposzczony
        private void Window_Drop(object sender, DragEventArgs e)
        {
            //string który posiada obiekt wrzucony przez uzytkownika 
            string filename = (string)((DataObject)e.Data).GetFileDropList()[0];

            //media.Source o typie URI określa co ma grac w tym przypadku to co wrzucił użytkownik pod nazwą filename
            mediaElement1.Source = new Uri(filename);

            //określenie zachowanie elemntu "mediaElement1" kiedy jest załadowany i nie jest
             //skrócona forma zapisu jeśli różnym rzeczą daje się tą samą wartość
             mediaElement1.LoadedBehavior = mediaElement1.UnloadedBehavior = MediaState.Manual;
             mediaElement1.Volume = (double)slider_vol.Value;

            //wbudowana metoda która powoduje odtworzenie elemntu, działa tutaj automatycznie od razu po upuszczniu pliku
            mediaElement1.Play();

            //nawiązanie połączenia z bazą i zapisania jego lokalizji (skróconej)
            sqlCon.IfDatabaseExist(baza);
            sqlCon.AddToSql(GetFileFolderName(filename));
            sqlCon.CloseConnection();
        }

        private void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSpan ts = mediaElement1.NaturalDuration.TimeSpan;
            slider_seek.Maximum = ts.TotalSeconds;
            timer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Przeciągnij plik, który chcesz odtworzyć");
        }

        //tą funkcje pisałem już kiedyś w innym projekcie który robiłem, czyli przeglądarke pliku w windowsie, nawet tych ukrytych (mogę Pani to przesłać, jeśli Pani chce )
        public static string GetFileFolderName(string path)
        {
            // C:\lokalizacja\plik 
            //jesli jest ścieżka jest pusta zwraca pustego stringa 
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            //akcja typu crossplatform (tutaj w wypadku MacOS), czyli zamienienie slashy na backslashe
            var normalizedPath = path.Replace('/', '\\'); //trzeba napisać "\\" dlatego, że "\" jest zajętym znakiem specjalnym, ale oznaczają to samo

            //znajdz ostatni backslash
            //lastIndex jest ostatnim slashem w dużym stringu, składającym się z mniejszych stringów zawierających poszczególne lokalizacje
            var lastIndex = normalizedPath.LastIndexOf('\\');

            //jesli nie ma żadnego backslasha zwara poprostu ścieżkę
            if (lastIndex <= 0)
            {
                return path;
            }

            //interesuje nas substring po ostanim backslashu, czyli dodajemy do "lastIndex" 1 by się do niego dostać i zwaracamy tą wartość
            return path.Substring(lastIndex + 1);
        }
        #endregion
    }
}
