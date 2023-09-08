using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Difficulty Difficulty { get; set; }
        public Game GameBoard { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Difficulty = Difficulty.Beginner;
            GameBoard = new(Difficulty);

            int row = GameBoard.Tiles.GetLength(0);
            int col = GameBoard.Tiles.GetLength(1);
            CreateGrid(row, col);



            /*
            ValueTuple<int, int, string> test = (5, 3, "mojn");

            (int, int, string) test2 = (5, 3, "mojn");

            var test3 = (5, 3, "mojn");

            var (x, y, s) = (5, 3, "mojn");

            string greet = s;
            */
        }


        private void CreateGrid(int row, int col)
        {
            if (row <= 0 || col <= 0)
            {
                throw new ArgumentException("Both columns and rows must be greater than zero.");
            }

            for (int i = 0; i < row; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int j = 0; j < col; j++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            AddButtons(row, col);
        }

        private void AddButtons(int row, int col)
        {
            for (int i = 0; i < row; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    ButtonXY button = new ButtonXY(i, j);
                    button.Style = (Style)FindResource("ButtonStyle");
                    button.Click += Button_Click;
                    Grid.SetColumn(button, j);
                    Grid.SetRow(button, i);
                    grid.Children.Add(button);

                    Tile currentTile = GameBoard.Tiles[i,j];
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonXY button = sender as ButtonXY;
            int x = button.X;
            int y = button.Y;

            GameBoard.RevealTiles(x, y);
            RevealAll();
        }

        private Button GetButtonAt(int row, int col)
        {
            foreach (UIElement element in grid.Children)
            {
                if (element is Button button && Grid.GetRow(button) == row && Grid.GetColumn(button) == col)
                {
                    return button;
                }
            }

            return null;
        }

        private void RevealAll()
        {
            int row = GameBoard.Tiles.GetLength(0);
            int col = GameBoard.Tiles.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (!GameBoard.Tiles[i, j].IsRevealed) continue;

                    ButtonXY button = (ButtonXY)GetButtonAt(i, j);
                    button.IsEnabled = false;

                    if (GameBoard.Tiles[i,j].HasBomb)
                    {
                        button.Content = "\uD83C\uDF4F";
                        button.SetImage("Images/Mine.png");
                        
                    }
                    else if (GameBoard.Tiles[i, j].AdjacentBombCount != 0)
                    {
                        button.Content = GameBoard.Tiles[i, j].AdjacentBombCount;
                    }
                }
            }
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            GameBoard.ResetBoard();
            grid.Children.Clear();
            AddButtons(GameBoard.Tiles.GetLength(0), GameBoard.Tiles.GetLength(1));
        }
    }
}
