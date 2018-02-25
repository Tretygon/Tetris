using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisGameManager
{

    public enum Shapes { Block, Г, L, Z, S, T, I , Preview};

    /// <summary>
    /// Logic Core of Tetris
    /// </summary>
    public class Game
    {
        #region Variables + Properties + Events
        private List<Cell> Tetromino;
        private List<Cell> Preview = new List<Cell>();
        private List<Cell> StationaryCells;
        private List<Cell> upcomingTetromino;
        private readonly Cell startCell;
        private Cell centerCell;
        private bool isPaused, isGameOver, bouncingOffWalls, showDropPreview;
        private int rows, collumns, speed, score, lines, level;
        private System.Timers.Timer fallTimer;
        
        public event Action<List<Cell>> OnDraw;
        public event Action<List<Cell>> OnErase;
        public event Action<List<Cell>> OnReDrawSideGrid;
        public event Action<List<Cell>> OnReDrawGameGrid;
        public event Action<int> OnUpdateScore;
        public event Action<int> OnUpdateLines;
        public event Action<int> OnUpdateLevel;
        public event Action<int> OnGameOver;
        
        public bool ShowDropPreview
        {
            get => showDropPreview;
            set
            {
                showDropPreview = value;

                if (value)
                { 
                    Preview?.Clear();
                    AdjustDropPreview();
                }
                else
                    OnErase?.Invoke(Preview.Except(Tetromino).ToList());
            }
        }
        public int Score
        {
            get => score;
            private set
            {
                score = value;
                if (!isGameOver)
                {
                    OnUpdateScore?.Invoke(score);
                }
                
            }
        }
        public int Lines
        {
            get => lines;
            private set
            {
                int linesCleared = value - lines;
                for (int i = 0; i < linesCleared/4; i++)
                {
                    Score += 1200 * (Level + 1);
                }
                 
                switch (linesCleared%4)
                {
                    case 1:
                        Score += 40 * (Level + 1);
                        break;
                    case 2:
                        Score += 100 * (Level + 1);
                        break;
                    case 3:
                        Score += 300 * (Level + 1);
                        break;
                    default:
                        break;
                }

                lines = value;
                Level = lines / 5;
                OnUpdateLines?.Invoke(lines);
            }
        }
        public int Level
        {
            get => level;
            private set
            {
                if (level != value)
                {
                    fallTimer.Interval *= Math.Pow(0.8, value-level);
                    level = value;
                    OnUpdateLevel?.Invoke(level);
                }
            }
        }
        public bool IsPaused
        {
            get => isPaused;
            set 
            {
                if (!isGameOver)
                {
                    isPaused = value;
                    if (isPaused)
                    {
                        fallTimer.Stop();
                    }
                    else
                    {
                        fallTimer.Start();
                    }
                };
            }
        }
        public bool IsGameOver => isGameOver; 
        #endregion
        
        public Game(int collumns, int rows, bool enableDropPreview)
        {
            this.collumns = collumns;
            this.rows = rows;
            centerCell = new Cell(collumns / 2, 0);
            startCell = new Cell(collumns / 2, 0);

            showDropPreview = enableDropPreview;
            fallTimer = new System.Timers.Timer();
            fallTimer.Elapsed += (a, b) => MoveDown();
            Tetromino = new List<Cell>();
            StationaryCells = new List<Cell>();
            bouncingOffWalls = true;
        }
        /// <summary>
        /// Starts a new game
        /// </summary>
        public void NewGame()
        {
            isGameOver = false;
            Tetromino?.Clear();
            StationaryCells?.Clear();
            Preview.Clear();
            GenerateTetromino();

            Score = 0;
            Lines = 0;
            Level = 0;
            speed = 750;
            fallTimer.Interval = speed;
            SpawnNewTetromino();
            IsPaused = false;
        }
        /// <summary>
        /// Ends this game
        /// </summary>
        private void GameOver()
        {
            IsPaused = true;
            isGameOver = true;
            OnGameOver?.Invoke(score);
        }
        #region movement
        /// <summary>
        /// Checks whether the proposed move is possible ->performs move by updating UI & fallingCells 
        /// </summary>
        /// <param name="movePreview">List of falling cells after the proposed move</param>
        /// <returns>Bool indicating whether attempt was succesful or not </returns>
        private bool DeployMoveProposal(List<Cell> movePreview)
        {
            if (!(movePreview.Exists(mp => mp.X < 0 || mp.X >= collumns || mp.Y >= rows || StationaryCells.Exists(sc => sc == mp))))        //rewrite
            {
                OnDraw?.Invoke(movePreview.Except(Tetromino).ToList());
                OnErase?.Invoke(Tetromino.Except(movePreview).ToList());
                Tetromino = movePreview;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to descent the Tetromino
        /// </summary>
        private void MoveDown()
        {
            List<Cell> movePreview = new List<Cell>();
            foreach (Cell c in Tetromino)
            {
                movePreview.Add(new Cell(c.X, c.Y + 1, c.Shape));
            }
            if (DeployMoveProposal(movePreview))
            {
                centerCell.Y++;
            }
            else
            {
                HandleCollision();
            }
        }

        /// <summary>
        /// Attempts to rotate the Tetromino
        /// </summary>
        public void Rotate()
        {
            if (IsPaused || Tetromino[0].Shape == Shapes.Block)
                return;

            List<Cell> rotationPreview = new List<Cell>();
            foreach (Cell c in Tetromino)
            {
                rotationPreview.Add(new Cell(centerCell.Y + centerCell.X - c.Y, c.X - centerCell.X + centerCell.Y, c.Shape));                 //  [X,Y] -> [-Y,X]
            }

            if (!DeployMoveProposal(rotationPreview))
                if (bouncingOffWalls)
                    if (!DeployMoveProposal(rotationPreview.ForEachWithReturn(c => c.X++)))                                       //bouncing off walls
                        if (!DeployMoveProposal(rotationPreview.ForEachWithReturn(c => c.X++)))
                            if (!DeployMoveProposal(rotationPreview.ForEachWithReturn(c => c.X -= 3)))
                                if (!DeployMoveProposal(rotationPreview.ForEachWithReturn(c => c.X--)))
                                    { }
                                else
                                {
                                    centerCell.X -= 2;
                                    AdjustDropPreview();
                                }
                            else
                            {
                                centerCell.X--;
                                AdjustDropPreview();
                            }
                        else
                        {
                            centerCell.X += 2;
                            AdjustDropPreview();
                        }
                    else
                    {
                        centerCell.X++;
                        AdjustDropPreview();
                    }
                else
                {
                }
            else
            {
                AdjustDropPreview();
            }
        }

        /// <summary>
        /// Attempts to descent the Tetromino
        /// </summary>
        public void Drop()
        {
            if (IsPaused)
                return;
            OnErase?.Invoke(Tetromino);
            while (!(Tetromino.Exists(mp => mp.Y >= rows-1 || StationaryCells.Exists(sc => sc.Y == mp.Y+1 && sc.X == mp.X))))
            {
                Tetromino.ForEach(c => c.Y++);
                Score += level+1;
            }
            OnErase?.Invoke(Preview.Except(Tetromino).ToList()); // to prevent a bug when after drop there sometimes remained preview cells
            OnDraw?.Invoke(Tetromino);
            HandleCollision();
        }

        /// <summary>
        /// Attempts to move the Tetromino to the Left
        /// </summary>
        public void MoveLeft()
        {
            if (IsPaused)
                return;
            List<Cell> movePreview = new List<Cell>();
            foreach (Cell c in Tetromino)
            {
                movePreview.Add(new Cell(c.X - 1, c.Y, c.Shape));
            }
            if (DeployMoveProposal(movePreview))
            {
                centerCell.X--;
                AdjustDropPreview();
            }
        }

        /// <summary>
        /// Attempts to move the Tetromino to the Right
        /// </summary>
        public void MoveRight()
        {
            if (IsPaused)
                return;
            List<Cell> movePreview = new List<Cell>();
            foreach (Cell c in Tetromino)
            {
                movePreview.Add(new Cell(c.X + 1, c.Y, c.Shape));
            }
            if (DeployMoveProposal(movePreview))
            {
                centerCell.X++;
                AdjustDropPreview();
            }
        }

        #endregion
        #region Clearing
        /// <summary>
        /// Checks all lines, clears any that are full
        /// </summary>
        public int ClearFullLines()
        {
            int linesRemoved = 0;
            for (int y = 0; y < rows; y++)
            {
                if (StationaryCells.Count(c => c.Y == y) == collumns)                                     //finds full line
                {   
                    StationaryCells.RemoveAll(c => c.Y == y);                                             //removes the line
                    StationaryCells.FindAll(c => c.Y < y).ForEach(c => c.Y++);                            //naive gravity
                    linesRemoved++;
                }
            }
            if (linesRemoved > 0)
            {
                Gravity();                                                                               //sticky gravity
                return linesRemoved + ClearFullLines();
            }
            return 0;
        }
        /// <summary>
        /// Divides stationary cells into segments, lets those segments fall
        /// </summary>
        /// <returns></returns>
        public void Gravity()                                                                       
        {
            List<Cell> all = new List<Cell>();
            List<List<Cell>> AllSegments = new List<List<Cell>>();
            
            foreach (Cell c in StationaryCells)                  //copies stationary cells
            {
                 all.Add(new Cell(c.X, c.Y, c.Shape));
            }

            while(all.Any())                                    //chops into segments
            {
                List<Cell> segment = new List<Cell>();
                Segmentate(all[0], segment, all);
                AllSegments.Add(segment);
            }

            bool terminateRecursion = false;
            while (!terminateRecursion)
            {
                terminateRecursion = true; 
                foreach (var segment in AllSegments)
                {
                    segment.ForEach(c => StationaryCells.Remove(c));
                    while (!(segment.Exists(c1 => c1.Y >= rows - 1 || StationaryCells.Exists(c2 => c2.Y == c1.Y + 1 && c2.X == c1.X))))                     //segment falling
                    {
                        segment.ForEach(c => c.Y++);
                        terminateRecursion = false;
                    }
                    segment.ForEach(c => StationaryCells.Add(c));
                }
            }
        }

        /// <summary>
        /// Flood fill algorithm
        /// </summary>
        public void Segmentate(Cell start, List<Cell> segment, List<Cell> all) 
        {
            if (start is null)
                return;
            segment.Add(start);
            all.Remove(start);
            
            Segmentate(all.Find(c => c.Y == start.Y && c.X == start.X + 1), segment, all);
            Segmentate(all.Find(c => c.Y == start.Y && c.X == start.X - 1), segment, all);
            Segmentate(all.Find(c => c.Y == start.Y +1 && c.X == start.X), segment, all);
            Segmentate(all.Find(c => c.Y == start.Y -1 && c.X == start.X), segment, all);
        }
        #endregion
        /// <summary>
        /// transfers tetromino to stationary ->clears full lines ->spawns new tetromino
        /// </summary>
        private void HandleCollision()
        {
            StationaryCells.AddRange(Tetromino);
            Tetromino?.Clear();  //if not cleared, the last tetromino, now stationary, will be visually erased in the deploy method called from spawn new
            Preview?.Clear();
            int linesRemoved = ClearFullLines();
            if (linesRemoved > 0)
            {
                Lines += linesRemoved;
                OnReDrawGameGrid?.Invoke(StationaryCells);
            }
            SpawnNewTetromino();
        }
        /// <summary>
        /// Spawns next Tetromino
        /// </summary>
        private void SpawnNewTetromino()
        {
            if (DeployMoveProposal(GenerateTetromino().ForEachWithReturn(c => { c.X += startCell.X; c.Y += startCell.Y; })))  //detects whether new tetromino can spawn and spawns it
            {
               AdjustDropPreview();
            }
            else
            {
               GameOver();
            }
            centerCell.X = startCell.X;
            centerCell.Y = startCell.Y;
        }
        
        /// <summary>
        /// returns upcoming tetromino, Generates a brand new upcomingTetromino
        /// </summary>
        private List<Cell> GenerateTetromino()
        {
            List<Cell> temp = upcomingTetromino;
            upcomingTetromino = TetrominoGenerator.Generate();
            OnReDrawSideGrid?.Invoke(upcomingTetromino);
            return temp;
        }

        /// <summary>
        /// calculates new DropPreview and draws it
        /// </summary>
        private void AdjustDropPreview()
        {
            if (!ShowDropPreview || !Tetromino.Any())
                return;
            List<Cell> Old = Preview;
            List<Cell> New = new List<Cell>();
            
            foreach (Cell cell in Tetromino)
            {
                New.Add(new Cell(cell.X, cell.Y, Shapes.Preview));
            }
            while (!(New.Exists(n => n.Y >= rows - 1 || StationaryCells.Exists(o => o.Y == n.Y + 1 && o.X == n.X))))
            {
                New.ForEach(c => c.Y++);
            }
            
            OnErase?.Invoke(Old.Except(Tetromino).Except(StationaryCells).Except(New).ToList());               
            OnDraw?.Invoke(New.Except(Tetromino).Except(StationaryCells).Except(Old).ToList());
            Preview = New;
        }

        
        public List<Cell> GetAllCells()
        {
            return Tetromino.Concat(StationaryCells).Union(Preview).ToList();
        }
        public List<Cell> GetSideCells()
        {
            return upcomingTetromino;
        }
        public void Pause()
        {
            IsPaused = !IsPaused;
        }
    }
}
