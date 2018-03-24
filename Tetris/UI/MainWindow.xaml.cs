using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.Windows.Interop;
//WindowState="Maximized"                           => fullscreen?
//WindowStyle="None"           

namespace Tetris
{
    public partial class MainWindow : Window
    {
        #region Variables + Properties + Events
        public readonly List<Player> Players;
        private bool isPaused = true;
        private bool stretchingInProgress = false;
        double maxWIndowWidth;
        public event Action<float> OnVolumeChanged;
        MediaPlayer MusicPlayer = new MediaPlayer();
        MediaPlayer NoisePlayer = new MediaPlayer();
        public List<SingleKeyBinding> MultiPlayerControls = new List<SingleKeyBinding>();
        private  List<MultiKeyBinding> SinglePlayerControls;
        public  Action<Key> KeyPress;


        public bool MultiplayerEnabled
        {
            get => App.Settings.multiplayerEnabled;
            set
            {
                Dispatcher.Invoke(() =>
                {
                    Players[1].IsEnabled = value;
                    App.Settings.multiplayerEnabled = Players[1].IsEnabled;
                    AdjustControls();
                    AdjustWidth();
                });
            }
        }
        public bool IsPaused
        {
            get => isPaused;
            set
            {
                Dispatcher.Invoke(() =>
                {
                    isPaused = value;
                    Players.ForEach(p => p.Pause(isPaused));
                    ChangeMusicPlayerState(!isPaused);
                    PauseButton.IsChecked = isPaused;
                    PauseButton.Content = isPaused ? "Resume" : "Pause";
                });
            }
        }
        public bool SoundEnabled
        {
            get => App.Settings.soundEnabled;
            set
            {
                App.Settings.soundEnabled = value;
                ChangeMusicPlayerState(value);
            }
        }
        #endregion
        
        /// <summary>
        /// constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            maxWIndowWidth += universe1.Width + universe2.Width + Options.Width + 15;
            OnVolumeChanged += (volume) => { NoisePlayer.Volume = volume; MusicPlayer.Volume = volume; };
            MusicPlayer.Open(App.Settings.MusicUri);            
            MusicPlayer.MediaEnded += (a, b) =>         //looping of music
            {
                MusicPlayer.Position = TimeSpan.Zero;
                MusicPlayer.Play();
            };

            Players = new List<Player>()
            {
                new Player(
                    App.Settings.player1Settings,
                    true,
                    this,
                    universe1,
                    GameGrid1, 
                    SideGrid1, 
                    ScoreLabel1, 
                    LevelLabel1, 
                    LinesLabel1
                ),
                new Player(
                    App.Settings.player2Settings,
                    App.Settings.multiplayerEnabled,
                    this,
                    universe2,
                    GameGrid2,
                    SideGrid2,
                    ScoreLabel2,
                    LevelLabel2,
                    LinesLabel2
                ),
            };
            VolumeSlider.Value = App.Settings.volume * 100;
            OnVolumeChanged?.Invoke(App.Settings.volume);
            window.Width = App.Settings.multiplayerEnabled ? maxWIndowWidth : maxWIndowWidth - universe2.Width;
            ToggleMusicButton.IsChecked = App.Settings.soundEnabled;
            MultiPlayerButton.IsChecked = App.Settings.multiplayerEnabled;
            DropPreviewButton.IsChecked = App.Settings.showDropPreview;
            LinesButton.IsChecked = App.Settings.ShowGridLines;
            FlipButton.IsChecked = App.Settings.flipped;
            ReverseButton.IsChecked = App.Settings.controlsReversed;
            AdjustControls();
        }
        /// <summary>
        /// begins new game
        /// </summary>
        public void NewGame()
        {
            MusicPlayer.Position = TimeSpan.Zero;
            IsPaused = false;
            Players.ForEach(p => p.NewGame());
        }
        /// <summary>
        /// checks whether game can be ended
        /// </summary>
        public void CheckForGameOver(int score)
        {
            if (Players.TrueForAll(p => p.IsGameOver || !p.IsEnabled || !Players.Where(p2 => p2 != p).Any(p2 => p.Score < p2.Score)))
            {
                Dispatcher.Invoke(() =>
                {
                    IsPaused = true;
                    var popUp = new PopUpWindow();
                    popUp.Content = new GameOverScreen(popUp);
                    popUp.Title = "Game over";
                    popUp.ShowDialog();
                });
            }
            else
                Players.ForEach(p => p.scoreToSurpass = score);
        }
        /// <summary>
        /// makes NoisePlayer play a sound from /Sounds Directoctory
        /// </summary>
        public void PlaySound(string file)
        {
            if (SoundEnabled)
            {
                Dispatcher.Invoke(() =>
                {
                    NoisePlayer.Open(new Uri(Environment.CurrentDirectory + $@"/Sound/{file}"));
                    NoisePlayer.Play();
                });
            }
        }
        /// <summary>
        /// to start or pause MusicPlayer
        /// </summary>
        public void ChangeMusicPlayerState(bool shouldBePlaying)
        {
            Dispatcher.Invoke(() =>
            {
                if (shouldBePlaying && SoundEnabled && !IsPaused)
                    MusicPlayer.Play();

                else
                    MusicPlayer.Pause();
            });
        }
        /// <summary>
        /// changes mainwindow width and thereby view on player2
        /// </summary>
        void AdjustWidth()
        {
            if (!stretchingInProgress)
            {
                DoubleAnimation AdjustWidth = new DoubleAnimation
                {
                    From = MultiplayerEnabled ? maxWIndowWidth - universe2.Width - 15 : maxWIndowWidth,
                    To = MultiplayerEnabled ? maxWIndowWidth : maxWIndowWidth - universe2.Width - 15,
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    AutoReverse = false
                };
                AdjustWidth.Completed += (a, b) => {
                    NewGame();
                    stretchingInProgress = false;
                };
                Dispatcher.Invoke(() =>
                {
                    stretchingInProgress = true;
                    IsPaused = true;
                    BeginAnimation(WidthProperty, AdjustWidth);
                    PlaySound(MultiplayerEnabled ? "SlideUp.mp3" : "SlideDown.mp3");
                });
            }
        }
        /// <summary>
        /// switches between multiplayer and singleplayer controls
        /// </summary>
        /// <Note>
        /// Needs to be called every time a Key is rebound
        /// </Note>
        public void AdjustControls()
        {
            if (MultiplayerEnabled)
            {
                KeyPress = (Key key) => MultiPlayerControls.Find(c => c.Key == key)?.Command();
            }
            else
            {
                Player EnabledPlayer = Players.Single(p => p.IsEnabled == true);
                SinglePlayerControls = new List<MultiKeyBinding>{
                        new MultiKeyBinding(
                        Players.Select(p =>p.Left.Key).ToList(),
                        EnabledPlayer.Left.Command
                        ),
                        new MultiKeyBinding(
                        Players.Select(p => p.Right.Key).ToList(),
                        EnabledPlayer.Right.Command                                     

                        ),
                        new MultiKeyBinding(
                        Players.Select(p => p.Rotate.Key).ToList(),
                        EnabledPlayer.Rotate.Command
                        ),
                        new MultiKeyBinding(
                        Players.Select(p => p.Drop.Key).ToList(),
                        EnabledPlayer.Drop.Command
                        ),
                    };
                KeyPress = (Key key) =>SinglePlayerControls.Find(c => c.Keys.Contains(key))?.Command();
            }
        }
        
        public void BurnDown()
        {
            Dispatcher.Invoke(async() =>
            {
                MusicPlayer.Volume = 0;
                PlaySound("Fire.mp3");
                MainGrid.Children.Clear();
                MainGrid.Background = new LinearGradientBrush(Colors.OrangeRed, Colors.DarkOrange, 90);
                await Task.Delay(2000);
                Application.Current.Shutdown();
            });
            
        }
        private void OpenMenu()
        {
            Dispatcher.Invoke(() =>
            {
                bool temp = IsPaused;
                IsPaused = true;
                new PopUpWindow{
                    Content = new MenuIndex(),
                    Title = "Menu",
                }.ShowDialog();
                IsPaused = temp;
            });
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Space)
                IsPaused = !IsPaused;
            else if (e.Key == Key.Escape)
                OpenMenu();
            else
                KeyPress(e.Key);
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            IsPaused = !IsPaused;
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }
        private void ReverseButton_Click(object sender, RoutedEventArgs e)
        {
            Players.ForEach(p => p.ReverseControls());
            AdjustControls();
            App.Settings.controlsReversed = !App.Settings.controlsReversed;
        }
        private void MultiPlayerButton_Click(object sender, RoutedEventArgs e)              
        {
            MultiplayerEnabled = !MultiplayerEnabled;
        }
        private void Fluff()
        {
            MessageBox.Show
                (
                    "╔╦╗╔═╗╔╦╗╦═╗╦╔═╗" + Environment.NewLine +
                    " ║ ║╣  ║ ╠╦╝║╚═╗" + Environment.NewLine +
                    " ╩ ╚═╝ ╩ ╩╚═╩╚═╝"
                );
        }
        
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MusicPlayer.Volume = 0;
            //App.Settings.Save();
        }
        
        private void ToggleSoundButton_Click(object sender, RoutedEventArgs e)
        {
            SoundEnabled = !SoundEnabled;
        }
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)(e.NewValue / 100);
            OnVolumeChanged?.Invoke(value);
            App.Settings.volume = value;
        }

        private void LinesButton_Click(object sender, RoutedEventArgs e)
        {
            Players.ForEach(p => p.ToggleGridLines());
            App.Settings.ShowGridLines = LinesButton.IsChecked.GetValueOrDefault();
        }

        private void DropPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            Players.ForEach(p => p.ToggleDropPreview());
            App.Settings.showDropPreview = DropPreviewButton.IsChecked.GetValueOrDefault();
        }

        private void FlipButton_Click(object sender, RoutedEventArgs e)
        {
            bool value = FlipButton.IsChecked.GetValueOrDefault();
            App.Settings.flipped = value;
            Players.ForEach(p => p.Flip());
            AdjustControls();
            FlipButton.Content = value ? "Flop" : "Flip";
            
        }

        private void FindMusicButton_Click(object sender, RoutedEventArgs e)
        {
            bool temp = IsPaused;
            IsPaused = true;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3| Wav files(*.wav)|*.wav|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                App.Settings.uriPath = openFileDialog.FileName;
                MusicPlayer.Open(App.Settings.MusicUri);
            }
            IsPaused = temp;        // to not resume if the game had been paused
        }
        ~MainWindow()
        {
            Environment.Exit(Environment.ExitCode);
        }
        
    }
}
