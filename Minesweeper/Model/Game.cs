using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Minesweeper
{
    public enum Difficulty
    {
        Beginner,
        Intermediate,
        Expert,
    }

    public class Game : INotifyPropertyChanged
    {
        public Tile[,] Tiles { get; private set; }
        private int Rows { get; set; }
        private int Cols { get; set; }
        private int BombCount { get; set; }

        private int _flagCount;

        public int FlagCount
        {
            get { return _flagCount; }
            set { _flagCount = value; OnPropertyChanged(); }
        }

        private Difficulty _selectedDifficulty;

        public Difficulty SelectedDifficulty
        {
            get { return _selectedDifficulty; }
            set
            {
                if (_selectedDifficulty != value)
                {
                    _selectedDifficulty = value;
                    OnPropertyChanged();
                    SetDifficulty();
                    ResetBoard();
                }
            }
        }


        #region Constructor

        // Constructor for default difficulties
        public Game(Difficulty diff)
        {
            SelectedDifficulty = diff;
            SetDifficulty();
            ResetBoard();
        }

        // Constructor for custom game
        public Game(int rows, int cols, int bombs)
        {
            Rows = rows;
            Cols = cols;
            BombCount = bombs;
            FlagCount = bombs;

            ResetBoard();
        }

        #endregion

        #region Binding

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Initializers
        private void SetDifficulty()
        {
            switch (SelectedDifficulty)
            {
                case Difficulty.Beginner:
                    Rows = 9;
                    Cols = 9;
                    BombCount = 10;
                    break;
                case Difficulty.Intermediate:
                    Rows = 16;
                    Cols = 16;
                    BombCount = 40;
                    break;
                case Difficulty.Expert:
                    Rows = 21;
                    Cols = 21;
                    BombCount = 99;
                    break;
            }
            FlagCount = BombCount;
        }

        // Fills the 2D array Tiles with Tile objects
        private void InitializeBoard()
        {
            this.Tiles = new Tile[Rows, Cols];
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    Tiles[i, j] = new Tile(false); // Initialize tiles without bombs
                }
            }
        }

        // Places a fixed number of bombs on random tiles
        private void RandomizeBombs()
        {
            int bombsRemaining = BombCount;

            Random rnd = new Random();
            while (bombsRemaining > 0)
            {
                int randomRow = rnd.Next(0, Rows);
                int randomCol = rnd.Next(0, Cols);

                if (!Tiles[randomRow, randomCol].HasBomb)
                {
                    Tiles[randomRow, randomCol].HasBomb = true;
                    bombsRemaining--;
                }
            }
        }

        // Count the number of adjacent bombs for all tiles and sets the property AdjacentBombCount with this value
        private void CountAllBombs()
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    if (!this.Tiles[i, j].HasBomb)
                    {
                        this.Tiles[i,j].AdjacentBombCount = CalculateBombScore(i, j);
                    }
                }
            }
        }

        // Calculates the adjacent bombs for a specific tile
        private int CalculateBombScore(int x, int y)
        {
            int bombScore = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int newX = x + dx;
                    int newY = y + dy;
                    if (ValidatePosition(newX, newY))
                    {
                        if (Tiles[newX, newY].HasBomb)
                        {
                            bombScore++;
                        }
                    }
                }
            }
            return bombScore;
        }

        #endregion


        // Checks if the x and y are within bounds of the gameboard
        private bool ValidatePosition(int x, int y)
        {
            return (x >= 0 && x < this.Rows && y >= 0 && y < this.Cols);
        }


        // Clears the Tiles array and initialized a new game
        public void ResetBoard()
        {
            InitializeBoard();
            RandomizeBombs();
            AdjustBombPlacement();
            CountAllBombs();
            FlagCount = BombCount;
        }

        // Decrements the flags (Encapsulation)
        public void PlaceFlag()
        {
            if (FlagCount > 0)
            {
                FlagCount--;
            }
        }

        // Increments the flag (Encapsulation)
        public void RemoveFlag()
        {
            FlagCount++;
        }

        // Moves bombs around to a new random tile, if a tile has more than 3 bombs adjacent. Makes the game easier by creating "islands" of bombs
        private void AdjustBombPlacement()
        {
            Random rnd = new Random();
            int maxIterations = 5;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                for (int i = 0; i < this.Rows; i++)
                {
                    for (int j = 0; j < this.Cols; j++)
                    {
                        if (!Tiles[i, j].HasBomb)
                        {
                            int bombScore = CalculateBombScore(i, j);
                            if (bombScore > 3)
                            {
                                int randomRow = rnd.Next(0, Rows);
                                int randomCol = rnd.Next(0, Cols);

                                bool temp = Tiles[i, j].HasBomb;
                                Tiles[i, j].HasBomb = Tiles[randomRow, randomCol].HasBomb;
                                Tiles[randomRow, randomCol].HasBomb = temp;
                            }
                        }
                    }
                }
            }
        }

        // Reveals all tiles recursively not already revealed, having no adjacent bombs and does not contain a bomb
        public void RevealTiles(int x, int y)
        {
            if (!ValidatePosition(x, y) || Tiles[x, y].IsRevealed)
            {
                return;
            }

            Tiles[x, y].IsRevealed = true;

            if (Tiles[x, y].AdjacentBombCount != 0 || Tiles[x, y].HasBomb)
            {
                return;
            }

            int[] xOffset = { -1, 0, 0, 1, -1, 1, 1, -1 };
            int[] yOffset = { 0, -1, 1, 0, -1, 1, -1, 1 };

            for (int k = 0; k < xOffset.Length; k++)
            {
                int newX = x + xOffset[k];
                int newY = y + yOffset[k];

                RevealTiles(newX, newY);
            }
        }

        // Reveals all tiles iteratively not already revealed, having no adjacent bombs and does not contain a bomb. Uses a Breadth-first algorithm
        public void RevealTilesBfs(int startX, int startY)
        {
            Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
            queue.Enqueue((startX, startY));

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                if (!ValidatePosition(x, y) || Tiles[x, y].IsRevealed || Tiles[x,y].IsFlagged)
                    continue;

                Tiles[x, y].IsRevealed = true;

                if (Tiles[x, y].AdjacentBombCount != 0 || Tiles[x, y].HasBomb)
                    continue;

                int[] xOffset = { -1, 0, 0, 1,};
                int[] yOffset = { 0, -1, 1, 0,};

                for (int k = 0; k < xOffset.Length; k++)
                {
                    int newX = x + xOffset[k];
                    int newY = y + yOffset[k];

                    queue.Enqueue((newX, newY));
                }
            }
        }
    }
}