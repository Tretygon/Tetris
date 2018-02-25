using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TetrisGameManager;


    
namespace Tetris
{
    public class Player
    {
        #region variables + properties
        private Game game;
        public string ID;
        private bool enabled, flipped;
        public int scoreToSurpass;
        private Grid GameGrid;
        private Grid SideGrid;
        private Grid universe;
        private Pallete pallete;
        private Rectangle[,] GameField;
        private Rectangle[,] SideField;
        private MainWindow wnd;
        private event Action OnAnulateScore;
        private PlayerSettings settings;
        
        public SingleKeyBinding Left { get; private set; }
        public SingleKeyBinding Right { get; private set; }
        public SingleKeyBinding Rotate { get; private set; }
        public SingleKeyBinding Drop { get; private set; }
        

        public int Score => game.Score;

        public bool IsGameOver => game.IsGameOver;

        public Pallete Pallete
        {
            get => pallete;
            internal set
            {
                pallete = value;
                settings.scheme = pallete.CurrentScheme;
                GameGrid.Background = App.Settings.ShowGridLines ? Pallete.GridLinesBrush : Pallete.EmptyCellBrush;
                UpdateGameField(game.GetAllCells());
                UpdateSideField(game.GetSideCells());
            }
        }
        public bool IsEnabled
        {
            get => enabled;
            set
            {
                if (value == false)
                {
                    Pause(true);
                    Task.Run(async() =>              
                    {
                        await Task.Delay(1000);          //waiting for stretching animation to finish
                        UpdateSideField(null);           //erasing the grids
                        UpdateGameField(null);
                        OnAnulateScore?.Invoke();
                    });
                }
                enabled = value;
            }
        }
        #endregion
        public Player (PlayerSettings settings, bool enabled, MainWindow window, Grid _universe, Grid gameGrid, Grid sideGrid, Label scoreLabel, Label levelLabel, Label linesLabel)
        {
            wnd = window;
            this.settings = settings;
            ID = settings.ID;
            game = new Game(App.Settings.collumns, App.Settings.rows, App.Settings.showDropPreview);
            this.enabled = enabled;
            
            this.GameGrid = gameGrid;
            GameGrid.RenderTransformOrigin = new Point(0.5, 0.5);
            SideGrid = sideGrid;
            universe = _universe;
            GameField = GridInitialize(gameGrid, App.Settings.collumns, App.Settings.rows);
            SideField = GridInitialize(sideGrid, 5, 5);
            SideGrid.Background = Brushes.Transparent;
            Pallete =new Pallete(settings.scheme);   


            game.OnDraw += (List<Cell> cells) => Draw(GameField, cells);
            game.OnErase += (List<Cell> cells) => Erase(GameField, cells);
            game.OnReDrawGameGrid += UpdateGameField;
            game.OnReDrawSideGrid += UpdateSideField;
            game.OnGameOver += wnd.CheckForGameOver;

            game.OnUpdateLevel += (int i) => UpdateLabel(i, levelLabel);
            game.OnUpdateLines += (int i) => UpdateLabel(i, linesLabel);

            game.OnUpdateScore += (int i) =>
            {
                UpdateLabel(i, scoreLabel);
                if (scoreToSurpass > 0 && i > scoreToSurpass)
                    wnd.CheckForGameOver(i);
            };
            OnAnulateScore += () =>
             {
                 UpdateLabel(0, levelLabel);
                 UpdateLabel(0, scoreLabel);
                 UpdateLabel(0, linesLabel);
             };

            Left = new SingleKeyBinding(settings.LeftKey , game.MoveLeft, settings);
            Right = new SingleKeyBinding(settings.RightKey, game.MoveRight, settings);
            Drop = new SingleKeyBinding(settings.DropKey, game.Drop, settings);
            Rotate = new SingleKeyBinding(settings.RotateKey, game.Rotate, settings);
            wnd.MultiPlayerControls.Add(Left);
            wnd.MultiPlayerControls.Add(Right);
            wnd.MultiPlayerControls.Add(Rotate);
            wnd.MultiPlayerControls.Add(Drop);

            if (App.Settings.controlsReversed)
                ReverseControls();
            flipped = false;    //keys are set to normal rotation
            Flip();
        }

        #region updating grids
        private void Erase(Rectangle[,] field, List<Cell> cells)
        {
            foreach (Cell c in cells)
            {
                if (c.Y >= 0)
                    wnd?.Dispatcher.Invoke(() => field[c.X, c.Y].Fill = Pallete.EmptyCellBrush);
            }
        }

        private void Draw(Rectangle[,] field, List<Cell> cells)
        {
            foreach (Cell c in cells)
            {
                if (c.Y >= 0)
                    wnd?.Dispatcher.Invoke(() => field[c.X, c.Y].Fill = Pallete[c.Shape]);
            }
        }

        private void UpdateSideField(List<Cell> cells)
        {
            
            for (int x = 0; x < SideField.GetLength(0); x++)
            {
                for (int y = 0; y < SideField.GetLength(1); y++)
                {
                    wnd?.Dispatcher.Invoke(() =>
                    {
                        SideField[x, y].Fill = Brushes.Transparent;
                    });
                }
            }

            if (cells != null)
            {
                foreach (Cell c in cells)
                {
                    wnd?.Dispatcher.Invoke(() => SideField[c.X + 2, c.Y + 2].Fill = Pallete[c.Shape]);
                }
            }
        }

        private void UpdateGameField(List<Cell> cells)
        {
            Brush[,] ToDraw = new Brush[GameField.GetLength(0), GameField.GetLength(1)];

            for (int x = 0; x < GameField.GetLength(0); x++)
            {
                for (int y = 0; y < GameField.GetLength(1); y++)
                {
                    ToDraw[x, y] = Pallete.EmptyCellBrush;
                }
            }

            if (cells != null)
            {
                foreach (Cell c in cells)
                {
                    if (c.Y >= 0)
                    {
                        ToDraw[c.X , c.Y ] = Pallete[c.Shape];
                    }
                }
            }

            for (int x = 0; x < GameField.GetLength(0); x++)
            {
                for (int y = 0; y < GameField.GetLength(1); y++)
                {
                    wnd?.Dispatcher.Invoke(() => GameField[x,y].Fill = ToDraw[x, y]);
                }
            }
        }
        #endregion
        
        /// <summary>
        /// makes KeyBindings exchangange their commands
        /// </summary>
        public void ReverseControls()
        {
            Action temp;

            temp = Left.Command;
            Left.Command = Right.Command;
            Right.Command = temp;

            temp = Rotate.Command;
            Rotate.Command = Drop.Command;
            Drop.Command = temp;
        }
        /// <summary>
        /// populates grid with rectangles
        /// </summary>
        private Rectangle[,] GridInitialize(Grid grid, int columns, int rows)
        {
            Rectangle[,] field = new Rectangle[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Margin = new Thickness(0.5),
                        Stretch = Stretch.Fill,
                    };

                    field[x, y] = rectangle;
                    Grid.SetColumn(rectangle, x);
                    Grid.SetRow(rectangle, y);
                    grid.Children.Add(rectangle);
                }
            }
            return field;
        }
        
        /// <summary>
        /// pauses the game
        /// </summary>
        /// <param name="b"></param>
        public void Pause(bool b)
        {
            if (IsEnabled)
                game.IsPaused = b;
        }
        /// <summary>
        /// starts a new game
        /// </summary>
        public void NewGame()
        {
            scoreToSurpass = 0;
            if (IsEnabled)
            {
                UpdateSideField(null);
                UpdateGameField(null);
                game.NewGame();
            }
        }
        private void UpdateLabel(int i, Label label)
        {
            wnd?.Dispatcher.Invoke(() =>
            {
                label.Content = i.ToString();
            });
        }
        /// <summary>
        /// shows/hides grid lines
        /// </summary>
        public void ToggleGridLines()
        {
            GameGrid.Background = App.Settings.ShowGridLines ?  Pallete.EmptyCellBrush : Pallete.GridLinesBrush;
        }
        /// <summary>
        /// shows/hides drop preview
        /// </summary>
        public void ToggleDropPreview()
        {
            game.ShowDropPreview = !game.ShowDropPreview;
        }
        /// <summary>
        /// uses rotate transform to flip the main grid, switches left and right keybinding commands 
        /// </summary>
        public void Flip()
        {
            if (flipped != App.Settings.flipped)
            {
                GameGrid.RenderTransform = new RotateTransform(App.Settings.flipped ? 180 : 0);
                ReverseControls();
                flipped = App.Settings.flipped;
            }
        }
    }
}
