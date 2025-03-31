using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        int[] _level1Waves = { 5, 10, 15 };
        bool _goLeft, _goRight;
        bool _isGameOver;
        bool _isRecoveringFromDmg;
        float _dmgRecoveryTimer = 2.5f;
        float _savedDmgRecoveryTimer;
        float _pShootingCooldown = 0.8f;
        float _savedPShootingCooldown;
        int _enemyImages;
        int _bulletTimer;
        int _bulletTimerLimit = 90;
        int _totalEnemies;
        int _enemySpeed = 4;
        int _livesAmount = 3;
        int _currentWave = 0;

        public gra()
        {
            InitializeComponent();

            _savedDmgRecoveryTimer = _dmgRecoveryTimer;
            _savedPShootingCooldown = _pShootingCooldown;

            Application.Current.MainWindow.Height = 500;
            Application.Current.MainWindow.Width = 800;

            _gameTimer.Tick += GameLoop;
            _gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            _gameTimer.Start();

            _playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/zdjecia/player.png"));
            Player.Fill = _playerSkin;

            MyCanvas.Focus();
            MakeEnemies(_level1Waves[0]);
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            _pShootingCooldown -= 0.1f;

            if(_pShootingCooldown < 0)
            {
                _pShootingCooldown = _savedPShootingCooldown;

                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetTop(newBullet, Canvas.GetTop(Player) + newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(Player) + Player.Width / 2 - newBullet.Width / 2);
                MyCanvas.Children.Add(newBullet);
            }

            WavesLeft.Content = "Pozostałe fale: " + (_level1Waves.Length - _currentWave - 1);

            RecoveryTimer();
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

            int enemiesDetected = 0;

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

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>()) {
                        if (y is Rectangle && (string)y.Tag == "enemy") {
                            Rect enemy = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(enemy)) {
                                _itemsToRemove.Add(x);
                                _itemsToRemove.Add(y);
                                _totalEnemies -= 1;
                                break;
                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    enemiesDetected++;
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + _enemySpeed);

                    if(Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox)) {
                        ProcessDmgTaken("Koniec gry! Zabił cię najeźdźca");
                    }
                }
                if (x is Rectangle && (string)x.Tag == "enemyBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);

                    if(Canvas.GetTop(x) > 840)
                    {
                        _itemsToRemove.Add(x);
                    }

                    Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyBulletHitBox)) {
                        ProcessDmgTaken("Koniec gry! Zabił cię wrogi pocisk");
                    }
                }
            }

            foreach (Rectangle i in _itemsToRemove)
            {
                MyCanvas.Children.Remove(i);
            }

            if(enemiesDetected < 1) {
                _currentWave++;

                if(_currentWave > _level1Waves.Length - 1)
                {
                    ShowGameOverScreen("Wygrałeś!");
                }
                else
                {
                    MakeEnemies(_level1Waves[_currentWave]);
                    _enemySpeed += 2;
                }
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

            if (e.Key == Key.Space) {
                Rectangle newBullet = new Rectangle {
                    Tag = "bullet",
                    Height = 30,
                    Width = 12,
                    Fill = Brushes.Blue,
                    Stroke = Brushes.White
                };

                Canvas.SetTop(newBullet, Canvas.GetTop(Player) + newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(Player) + Player.Width / 2 - newBullet.Width / 2);
                MyCanvas.Children.Add(newBullet);
            }

            if (e.Key == Key.Enter && _isGameOver) {
                Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                Application.Current.Shutdown();
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

        private void RecoveryTimer()
        {
            if (_isRecoveringFromDmg)
            {
                _dmgRecoveryTimer -= 0.1f;

                if (_dmgRecoveryTimer < 0)
                {
                    _isRecoveringFromDmg = false;
                    _dmgRecoveryTimer = _savedDmgRecoveryTimer;
                }
            }

            LivesAmount.Content = "Ilość żyć: " + _livesAmount;
        }

        private void ProcessDmgTaken(string msg)
        {
            if (_isRecoveringFromDmg) { return; }

            if(_livesAmount > 1)
            {
                _livesAmount--;
                _isRecoveringFromDmg = true;
            }
            else
            {
                ShowGameOverScreen(msg);
            }
        }

        private void ShowGameOverScreen(string msg)
        {
            _isGameOver = true;
            _gameTimer.Stop();
            LivesAmount.Content = msg + "   Naciśnij Enter aby zagrać ponownie";
        }
    }
}
