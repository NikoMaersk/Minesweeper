using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Difficulty difficulty;
        private Game gameBoard;
        private DispatcherTimer gameTimer;
        private DateTime elapsedTime;

        public MainWindow()
        {
            InitializeComponent();
            difficulty = Difficulty.Intermediate;
            gameBoard = new(difficulty);

            int row = gameBoard.Tiles.GetLength(0);
            int col = gameBoard.Tiles.GetLength(1);
            CreateGrid(row, col);

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
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

                    Tile currentTile = gameBoard.Tiles[i,j];
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }

            ButtonXY button = sender as ButtonXY;
            int x = button.X;
            int y = button.Y;

            
            await Task.Run(() =>
            {
                gameBoard.RevealTilesBfs(x, y);

                Dispatcher.Invoke(() =>
                {
                    RevealAll();
                });
            });
            

            
            /*
            gameBoard.RevealTiles(x, y);
            RevealAll();
            */
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
            int row = gameBoard.Tiles.GetLength(0);
            int col = gameBoard.Tiles.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (!gameBoard.Tiles[i, j].IsRevealed) continue;

                    ButtonXY button = (ButtonXY)GetButtonAt(i, j);
                    button.IsEnabled = false;

                    if (gameBoard.Tiles[i,j].HasBomb)
                    {
                        button.Content = "\uD83C\uDF4F";
                        //button.SetImage("Images/Mine.png");
                        
                    }
                    else if (gameBoard.Tiles[i, j].AdjacentBombCount != 0)
                    {
                        button.Content = gameBoard.Tiles[i, j].AdjacentBombCount;
                    }
                }
            }
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            GameOver();
            grid.Children.Clear();
            AddButtons(gameBoard.Tiles.GetLength(0), gameBoard.Tiles.GetLength(1));
        }

      

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            LbTimer.Content = elapsedTime.ToString(@"mm\:ss");
        }

        // TO DO
        #region start/end game
        private void Start()
        {

        }

        private void GameOver()
        {
            gameTimer.Stop();
            elapsedTime = DateTime.MinValue;
            LbTimer.Content = "00:00";
            gameBoard.ResetBoard();
        }
        #endregion

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
        }
    }
}
