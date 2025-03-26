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
        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Kod do uruchomienia nowej gry
            MessageBox.Show("Nowa gra rozpoczęta!");
        }

        // Przycisk Instrukcje
        private void InstructionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Kod do pokazania instrukcji
            MessageBox.Show("Instrukcje gry");
        }

        // Przycisk Wyniki
        private void HighscoreButton_Click(object sender, RoutedEventArgs e)
        {
            // Kod do pokazania wyników
            MessageBox.Show("Wyniki gry");
        }

        // Przycisk Wyjście
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            // Kod do zamknięcia gry
            Application.Current.Shutdown();
        }

        private void nowa_gra(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new gra());
        }

        private void instrukcje_click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new instrukcje());
            
        }

        private void wyjdz_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
