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

namespace Tetris
{
    public partial class GameOverScreen : UserControl
    {
        public GameOverScreen(Window parent)
        {
            InitializeComponent();
            ExitButton.Click += (a, b) => Application.Current.Shutdown();
            RestartButton.Click += (a, b) => parent.Close(); 
            parent.Closing += (a,b) => App.wnd.NewGame();

            List<Player> winners = App.wnd.Players.Where(p2 => p2.Score == App.wnd.Players.Max(p => p.Score)).ToList();

            if (winners.Count==1)
            {
                Player winner = winners.First();
                
                if (winner.Score == 0)
                {
                    Loaded += (a, b) => App.wnd.PlaySound("SadMusic.wav");
                }
                else if (winner.Score < 1000)
                {
                    Loaded += (a, b) => App.wnd.PlaySound("Defeat.wav");
                }
                else if (winner.Score < 3000)
                {
                    Loaded += (a, b) => App.wnd.PlaySound("Victory.wav");
                }
                else if (winner.Score < 5000)
                {
                    Loaded += (a, b) => App.wnd.PlaySound("YAY.wav");
                }
                else
                {
                    Loaded += (a, b) => App.wnd.PlaySound("Champions.wav");
                }

                if (winner.Score == 0)
                {
                    text.Content = "Game Over" + Environment.NewLine + Environment.NewLine + $"Everyone lost. Nobody even tried :'(";
                }
                else if (App.wnd.Players.Where(p => p.IsEnabled).Count() == 1)
                {
                    text.Content = "Game Over" + Environment.NewLine + Environment.NewLine + $"You achieved {winner.Score} Score!";
                }
                else
                {
                    text.Content = "Game Over" + Environment.NewLine + Environment.NewLine + $"Winner: {winner.ID} with {winner.Score} Score!";
                }
            }
            else if (winners.Count ==0)
            {
                throw new Exception();
            }
            else
            {
                text.Content = "Game Over" + Environment.NewLine + Environment.NewLine + $"Winners:";
                foreach (var winner in winners)
                {
                    text.Content += Environment.NewLine + $"{winner.ID} with {winner.Score} Score!";
                } 
            }
            
        }
    }
}
