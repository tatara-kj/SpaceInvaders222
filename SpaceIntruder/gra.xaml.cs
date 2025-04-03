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
        List<Rectangle> _activeEnemies = new List<Rectangle>();
        Dictionary<Rectangle, Enemy> _enemies = new Dictionary<Rectangle, Enemy>();
        Dictionary<Rectangle, EnemyChaser> _enemyChasers = new Dictionary<Rectangle, EnemyChaser>();
        Rectangle _chargedSpecial = new Rectangle();
        int[] _level1Waves = { 30, 20, 30 };
        bool _goLeft, _goRight;
        bool _isGameOver;
        bool _isRecoveringFromDmg;
        bool _isChargingSpecial;
        bool _readyToReleaseSpecial;
        bool _isSpecialAvailable = true;
        float _dmgRecoveryTimer = 2.5f;
        float _savedDmgRecoveryTimer;
        float _pShootingCooldown = 1f;
        float _savedPShootingCooldown;
        float _pSpecialChargeTime = 3f;
        float _savedPSpecialChargeTime;
        int _enemyImages;
        int _bulletTimer;
        int _bulletTimerLimit = 70;
        int _totalEnemies;
        int _enemySpeed = 8;
        int _livesAmount = 3;
        int _currentWave = 0;

        public gra()
        {
            InitializeComponent();

            _savedDmgRecoveryTimer = _dmgRecoveryTimer;
            _savedPShootingCooldown = _pShootingCooldown;
            _savedPSpecialChargeTime = _pSpecialChargeTime;

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
            WavesLeft.Content = "Pozostałe fale: " + (_level1Waves.Length - _currentWave - 1);

            ChargeSpecialBullet();
            AutoShooting();
            RecoveryTimer();

            if (_goLeft && Canvas.GetLeft(Player) > 0)
            {
                if(_isChargingSpecial && _isSpecialAvailable) {
                    Canvas.SetLeft(Player, Canvas.GetLeft(Player) - 5);
                }
                else {
                    Canvas.SetLeft(Player, Canvas.GetLeft(Player) - 10);
                }
            }
            if (_goRight && Canvas.GetLeft(Player) + 80 < Application.Current.MainWindow.Width)
            {
                if (_isChargingSpecial && _isSpecialAvailable) {
                    Canvas.SetLeft(Player, Canvas.GetLeft(Player) + 5);
                }
                else {
                    Canvas.SetLeft(Player, Canvas.GetLeft(Player) + 10);
                }
            }

            Rect playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player) + 10, Player.Width, Player.Height - 10);

            _bulletTimer -= 3;
            Random rand = new Random();

            if(_bulletTimer < 0)
            {
                if(_activeEnemies.Count() < 0) { return; }

                Rectangle randomEnemy = _activeEnemies[rand.Next(0, _activeEnemies.Count())];

                if(Canvas.GetTop(randomEnemy) < 320) {
                    EnemyBulletMaker(Canvas.GetLeft(randomEnemy) + randomEnemy.Width / 2, Canvas.GetTop(randomEnemy), Canvas.GetTop(randomEnemy) < 260);
                }

                _bulletTimer = _bulletTimerLimit;
            }

            int enemiesDetected = 0;

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && ((string)x.Tag == "bullet" || (string)x.Tag == "specialBullet"))
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if(Canvas.GetTop(x) < 0)
                    {
                        _itemsToRemove.Add(x);

                        if((string)x.Tag == "specialBullet"){
                            _isSpecialAvailable = true;
                        }
                    }

                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>()) {
                        if (y is Rectangle && (string)y.Tag == "enemy") {
                            Rect enemy = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(enemy)) {
                                if ((string)x.Tag == "bullet") {
                                    _itemsToRemove.Add(x);
                                }

                                int enemyHp = _enemies[y].HP;

                                if (enemyHp < 2 || (string)x.Tag == "specialBullet")
                                {
                                    _itemsToRemove.Add(y);
                                    _activeEnemies.Remove(y);
                                    _enemies.Remove(y);
                                    enemiesDetected--;
                                }
                                else
                                {
                                    enemyHp--;
                                    _enemies[y].HP = enemyHp;
                                }

                                break;
                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    enemiesDetected++;
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + _enemySpeed * _enemies[x].MoveDirection);

                    if(_enemies[x].MoveDirection == 1 && Canvas.GetLeft(x) > 820)
                    {
                        // Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 8));
                        _enemies[x].MoveDirection = -1;
                    }
                    else if(_enemies[x].MoveDirection == -1 && Canvas.GetLeft(x) < 0) {
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 8));
                        _enemies[x].MoveDirection = 1;
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox)) {
                        ProcessDmgTaken("Koniec gry! Zabił cię najeźdźca");
                    }
                }

                if (x is Rectangle && ((string)x.Tag == "enemyBullet" || (string)x.Tag == "enemyChaser"))
                {
                    if((string)x.Tag == "enemyChaser") {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + (_enemyChasers[x].SavedPlayerLeftPos - Canvas.GetLeft(x)) / 10);
                        Canvas.SetTop(x, Canvas.GetTop(x) + 8);
                    }
                    else {
                        Canvas.SetTop(x, Canvas.GetTop(x) + 10);
                    }

                    if (Canvas.GetTop(x) > 840)
                    {
                        _itemsToRemove.Add(x);
                    }

                    Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyBulletHitBox)) {
                        _itemsToRemove.Add(x);
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
                    _enemySpeed += 2;
                    _bulletTimerLimit -= 10;
                    MakeEnemies(_level1Waves[_currentWave]);
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
                if (_readyToReleaseSpecial) { return; }

                _isChargingSpecial = true;
            }

            if (e.Key == Key.Enter) {
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

            if (e.Key == Key.Space) {
                _isChargingSpecial = false;
                _pSpecialChargeTime = _savedPSpecialChargeTime;

                if (_readyToReleaseSpecial) {
                    _chargedSpecial.Tag = "specialBullet";
                    _readyToReleaseSpecial = false;
                }
            }
        }

        private void EnemyBulletMaker(double x, double y, bool canMakeChaser)
        {
            Random rand = new Random();
            Rectangle enemyBullet = new Rectangle();

            if(rand.Next(0, 5) == 0 && canMakeChaser) {
                enemyBullet = new Rectangle {
                    Tag = "enemyChaser",
                    Height = 20,
                    Width = 20,
                    Fill = Brushes.OrangeRed,
                    Stroke = Brushes.Black,
                    StrokeThickness = 3
                };

                _enemyChasers.Add(enemyBullet, new EnemyChaser((float)(Canvas.GetLeft(Player) + Player.Width / 2 - 10)));
                Panel.SetZIndex(enemyBullet, 18);
            }
            else {
                enemyBullet = new Rectangle {
                    Tag = "enemyBullet",
                    Height = 40,
                    Width = 15,
                    Fill = Brushes.Yellow,
                    Stroke = Brushes.Black,
                    StrokeThickness = 3
                };

                Panel.SetZIndex(enemyBullet, 17);
            }

            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);
            MyCanvas.Children.Add(enemyBullet);
        }

        private void MakeEnemies(int limit)
        {
            int left = 0;

            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = new ImageBrush();

                Rectangle newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin
                };

                Canvas.SetTop(newEnemy, 10);
                Canvas.SetLeft(newEnemy, left);
                Panel.SetZIndex(newEnemy, 15);
                MyCanvas.Children.Add(newEnemy);
                _activeEnemies.Add(newEnemy);
                _enemies.Add(newEnemy, new Enemy());
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

        private void ChargeSpecialBullet() {
            if (_isChargingSpecial && _isSpecialAvailable) {
                _pSpecialChargeTime -= 0.1f;

                if (_pSpecialChargeTime < 0) {
                    _isChargingSpecial = false;
                    _isSpecialAvailable = false;
                    _pSpecialChargeTime = _savedPSpecialChargeTime;

                    _chargedSpecial = new Rectangle {
                        Tag = "chargedSpecial",
                        Height = 30,
                        Width = 12,
                        Fill = Brushes.Blue,
                        Stroke = Brushes.White
                    };

                    Canvas.SetTop(_chargedSpecial, Canvas.GetTop(Player) + _chargedSpecial.Height);
                    Canvas.SetLeft(_chargedSpecial, Canvas.GetLeft(Player) + Player.Width / 2 - _chargedSpecial.Width / 2);
                    Panel.SetZIndex(_chargedSpecial, 12);
                    MyCanvas.Children.Add(_chargedSpecial);
                    _readyToReleaseSpecial = true;
                }
            }
        }

        private void AutoShooting() {
            _pShootingCooldown -= 0.1f;

            if (_pShootingCooldown < 0) {
                _pShootingCooldown = _savedPShootingCooldown;

                Rectangle newBullet = new Rectangle {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetTop(newBullet, Canvas.GetTop(Player) + newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(Player) + Player.Width / 2 - newBullet.Width / 2);
                Panel.SetZIndex(newBullet, 11);
                MyCanvas.Children.Add(newBullet);
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
