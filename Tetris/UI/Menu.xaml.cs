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
    /// <summary>
    /// Interaction logic for MenuIndex.xaml
    /// </summary>
    public partial class MenuIndex : UserControl
    {
        public MenuIndex()
        {
            InitializeComponent();
        }
        private void BurnButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as PopUpWindow).Close();
            App.wnd.BurnDown();
        }

        private void CustomizeButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as PopUpWindow).Content = new CustomisePlayersScreen(Parent as PopUpWindow);
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as PopUpWindow).Close();
        }
    }
}
