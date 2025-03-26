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
    /// Logika interakcji dla klasy instrukcje.xaml
    /// </summary>
    public partial class instrukcje : Page
    {
        public instrukcje()
        {
            InitializeComponent();
        }

        private void powrot_menu(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new glowna());
        }
    }
}
