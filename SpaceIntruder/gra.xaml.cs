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
        List<Rectangle> _itemsToRemove = new List<Rectangle>();
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

            Application.Current.MainWindow.Height = 500;
            Application.Current.MainWindow.Width = 800;

            _gameTimer.Tick += GameLoop;
            _gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            _gameTimer.Start();

            _playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/player.png"));
            Player.Fill = _playerSkin;

            MyCanvas.Focus();
            MakeEnemies(10);
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            Rect playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);

            if (_goLeft && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - 10);
            }
            if (_goRight && Canvas.GetLeft(Player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + 10);
            }

            _bulletTimer -= 3;

            if(_bulletTimer < 0)
            {
                EnemyBulletMaker(Canvas.GetLeft(Player) + 20, 10);
                _bulletTimer = _bulletTimerLimit;
            }

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if(Canvas.GetTop(x) < 10)
                    {
                        _itemsToRemove.Add(x);
                    }

                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + _enemySpeed);

                    if(Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                }
                if (x is Rectangle && (string)x.Tag == "enemyBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);

                    if(Canvas.GetTop(x) > 840)
                    {
                        _itemsToRemove.Add(x);
                    }

                    Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                }
            }

            foreach (Rectangle i in _itemsToRemove)
            {
                MyCanvas.Children.Remove(i);
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                _goLeft = true;
            }
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                _goRight = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                _goLeft = false;
            }
            if (e.Key == Key.Right || e.Key == Key.D)
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
            Rectangle enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 40,
                Width = 15,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 5
            };

            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);
            MyCanvas.Children.Add(enemyBullet);
        }

        private void MakeEnemies(int limit)
        {
            int left = 0;
            _totalEnemies = limit;

            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = new ImageBrush();

                Rectangle newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin,
                };

                Canvas.SetTop(newEnemy, 10);
                Canvas.SetLeft(newEnemy, left);
                MyCanvas.Children.Add(newEnemy);
                left -= 60;

                _enemyImages++;

                if (_enemyImages > 8)
                {
                    _enemyImages = 1;
                }

                switch (_enemyImages)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/invader1.gif"));
                        break;
                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/invader2.gif"));
                        break;
                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/invader3.gif"));
                        break;
                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/invader4.gif"));
                        break;
                    case 5:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/invader5.gif"));
                        break;
                    case 6:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/invader6.gif"));
                        break;
                    case 7:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/invader7.gif"));
                        break;
                    case 8:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/invader8.gif"));
                        break;
                }
            }
        }

        private void ShowGameOverScreen(string msg)
        {

        }
    }
}
