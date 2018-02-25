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
    /// Interaction logic for CustomizePlayers.xaml
    /// </summary>
    public partial class CustomisePlayersScreen : UserControl
    {
        PopUpWindow parent;
        bool buttonInUse = false;
        int orderNum = 0;
        /// <summary>
        /// a place where players can customize themelves
        /// </summary>
        public CustomisePlayersScreen(PopUpWindow _parent)
        {
            parent = _parent;
            InitializeComponent();
            int row = 0;
            Grid griddy = new Grid() {Focusable = true };
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(griddy, row++);
            MainGrid.Children.Add(griddy);

            griddy.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < 7; i++)
            {
                griddy.RowDefinitions.Add(new RowDefinition());
            }

            Button returnButton = new Button() { Content = "Back" };
            returnButton.Click += (a, b) =>
            {
                if (buttonInUse)
                    return;
                parent.Content = new MenuIndex();
            };
            Grid.SetColumn(returnButton, 1);
            Grid.SetRow(returnButton, orderNum);
            griddy.Children.Add(returnButton);

            Button RestoreDefaultButton = new Button() { Content = "Restore Default"};
            RestoreDefaultButton.Click += (a, b) =>
            {
                if (buttonInUse)
                    return;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    App.RestoreDefaultSettings();
                    parent.Close();
                });
                
                
            };
            Grid.SetColumn(RestoreDefaultButton, 2);
            Grid.SetRow(RestoreDefaultButton, orderNum);
            griddy.Children.Add(RestoreDefaultButton);


            orderNum++;
            AddLabel("Name");
            AddLabel("Pallete");
            AddLabel("Move Left");
            AddLabel( "Move Right");
            AddLabel("Rotate");
            AddLabel("Drop");


            int playernum = 0;
            foreach (Player player in App.wnd.Players)
            {
                orderNum = 1;
                playernum++;
                griddy.ColumnDefinitions.Add(new ColumnDefinition());
                
                TextBox textBox = new TextBox() { TextWrapping = TextWrapping.NoWrap, Text = player.ID, Margin = new Thickness(15), AcceptsReturn = false, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center};
                textBox.TextChanged += (a,b) => player.ID = textBox.Text;
                Grid.SetColumn(textBox, playernum);
                Grid.SetRow(textBox, orderNum++);
                griddy.Children.Add(textBox);

                griddy.Children.Add(new Button() { Width = 0, Height = 0,  IsDefault = true,Focusable = true, Content = "", Background = Brushes.Transparent, }); //this stupid abomination is here so user can stop writing in textbox by pressing enter ...
                               

                ComboBox comboBox = new ComboBox() { ItemsSource = Enum.GetValues(typeof(Pallete.Scheme)), SelectedItem = player.Pallete.CurrentScheme, Margin = new Thickness(15), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Focusable = false };
                comboBox.SelectionChanged += (a, b) => player.Pallete = new Pallete((Pallete.Scheme)(comboBox.SelectedItem));
                Grid.SetColumn(comboBox, playernum);
                Grid.SetRow(comboBox, orderNum++);
                griddy.Children.Add(comboBox);

                AddChangeKeyButton(player.Left);
                AddChangeKeyButton(player.Right);
                AddChangeKeyButton(player.Rotate);
                AddChangeKeyButton(player.Drop);
            }





            void AddLabel(string text)
            {
                Border label = new Border() { Child = new Label() { Content = text} , Background = Brushes.WhiteSmoke, BorderBrush = Brushes.Transparent, Margin = new Thickness(50,15,50,15), CornerRadius = new CornerRadius(4)};
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, orderNum++);
                griddy.Children.Add(label);
            }
            void AddChangeKeyButton(SingleKeyBinding keyBinding)
            {
                Button button = new Button() { Content = keyBinding.Key.ToString() };
                button.Click += (o, b) => ChangeKey(o as Button, keyBinding);
                Grid.SetColumn(button, playernum);
                Grid.SetRow(button, orderNum++);
                griddy.Children.Add(button);
            }
            void ChangeKey(Button button, SingleKeyBinding keyBinding)
            {
                if (buttonInUse)
                    return;
                buttonInUse = true;
                button.Content = "Press a Key";
                parent.KeyDown += bind;
                void bind (object obj, KeyEventArgs args)
                {
                    if ((args.Key == Key.Escape || args.Key == Key.Space || args.Key == keyBinding.Key))
                    {
                        button.Content = keyBinding.Key.ToString();
                    }
                    else
                    {
                        if (!(App.wnd.MultiPlayerControls.FindAll(kb => kb != keyBinding).Any(kb => kb.Key==args.Key)))
                        {
                            keyBinding.Key = args.Key;
                            button.Content = args.Key.ToString();
                            App.wnd.AdjustControls();
                        }
                        else
                        {
                            button.Content = keyBinding.Key.ToString();
                        }
                    }
                    parent.KeyDown -= bind;
                    buttonInUse = false;
                } 
                
            }        
        }
    }
}
