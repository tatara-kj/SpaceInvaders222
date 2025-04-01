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
using PathIO = System.IO.Path;  // Alias dla System.IO.Path
using System.IO;
using System.Security.Cryptography;
using System.IO;
namespace SpaceIntruder
{
    /// <summary>
    /// Logika interakcji dla klasy glowna.xaml
    /// </summary>
    public partial class glowna : Page
    {
        public glowna()
        {
        
            InitializeComponent();
       
        }
        private MediaPlayer player = new MediaPlayer();
        private void OdtworzDzwiek()
        {
        

            string sciezka = PathIO.Combine(AppDomain.CurrentDomain.BaseDirectory, "sounds", "buttonClick.wav");


            if (File.Exists(sciezka))
            {
                player.Stop();
                player.Volume = 0.5 * 1.2;
                player.Open(new Uri(sciezka, UriKind.Absolute));
                player.Play();
            }
            else
            {
                MessageBox.Show("Plik dźwiękowy nie istnieje!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void nowa_gra(object sender, RoutedEventArgs e)
        {
            
            NavigationService.Navigate(new gra());
            OdtworzDzwiek();
        }

        private void instrukcje_click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new instrukcje());
            OdtworzDzwiek();

        }

        private void wyjdz_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            OdtworzDzwiek();
        }
    }
}
