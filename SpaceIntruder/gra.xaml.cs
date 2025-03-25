using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using System.Windows.Threading;

namespace SpaceIntruder
{
    /// <summary>
    /// Logika interakcji dla klasy gra.xaml
    /// </summary>
    public partial class gra : Page
    {
        DispatcherTimer _gameTimer = new DispatcherTimer();
        ImageBrush _playerSkin = new ImageBrush();
        List<Rectangle> _itemToRemove = new List<Rectangle>();
        bool _goLeft, _goRight;
        bool _isGameOver;
        int _enemyImages;
        int _bulletTimer;
        int _bulletTimerLimit = 90;
        int _totalEnemies;
        int _enemySpeed = 6;

        public gra()
        {
            InitializeComponent();

            _gameTimer.Tick += GameLoop;
            _gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            _gameTimer.Start();

            _playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/player.png"));
            Player.Fill = _playerSkin;

            MyCanvas.Focus();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (_goLeft && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - 10);
            }
            if (_goRight && Canvas.GetLeft(Player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + 10);
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                _goRight = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                _goRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetTop(newBullet, Canvas.GetTop(Player) + newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(Player) + Player.Width / 2);
                MyCanvas.Children.Add(newBullet);
            }
        }

        private void EnemyBulletMaker(double x, double y)
        {

        }

        private void MakeEnemies(double limit)
        {

        }

        private void ShowGameOverScreen(string msg)
        {

        }
    }
}
