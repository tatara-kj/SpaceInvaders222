using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logika interakcji dla klasy instrukcje.xaml
    /// </summary>
    public partial class instrukcje : Page
    {
        public instrukcje()
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
        private void powrot_menu(object sender, RoutedEventArgs e)
        {
            OdtworzDzwiek();    
            NavigationService.Navigate(new glowna());
        }
    }
}
