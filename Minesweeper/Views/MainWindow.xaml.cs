using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            difficulty = Difficulty.Beginner;
            gameBoard = new(difficulty);
            

            int row = gameBoard.Tiles.GetLength(0);
            int col = gameBoard.Tiles.GetLength(1);
            CreateGrid(row, col);

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;

            DataContext = gameBoard;
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
                    button.PreviewMouseDown += Button_PreviewMouseDown;
                    Grid.SetColumn(button, j);
                    Grid.SetRow(button, i);
                    grid.Children.Add(button);

                    Tile currentTile = gameBoard.Tiles[i, j];
                }
            }
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }

            ButtonXY button = sender as ButtonXY;
            int x = button.X;
            int y = button.Y;


            if (e.ChangedButton == MouseButton.Right)
            {
                if (!gameBoard.Tiles[x, y].hasFlag && gameBoard.FlagCount > 0)
                {
                    DrawFlag(button);
                }
                else if (gameBoard.Tiles[x, y].hasFlag)
                {
                    RemoveFlag(button);
                }
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                if (gameBoard.Tiles[x, y].HasMine && !gameBoard.Tiles[x,y].hasFlag)
                {
                    RevealAll();
                    GameOver();
                    button.Background = Brushes.Red;
                    ShowLoseDialog();
                }

                gameBoard.RevealTilesBfs(x, y);
                RevealAll();

                if (CheckWinCondition())
                {
                    ShowWinDialog();
                    GameOver();
                }
            }
        }

        public void DrawFlag(ButtonXY button)
        {
            string imageName = "Flag.png";
            button.SetImage(imageName);
            gameBoard.PlaceFlag();
            gameBoard.Tiles[button.X, button.Y].hasFlag = true;
        }

        public void RemoveFlag(ButtonXY button)
        {
            button.Content = "";
            gameBoard.RemoveFlag();
            gameBoard.Tiles[button.X, button.Y].hasFlag = false;
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

                    if (gameBoard.Tiles[i, j].HasMine)
                    {
                        button.SetImage("Mine.png");
                    }
                    else if (gameBoard.Tiles[i, j].AdjacentMineCount != 0)
                    {
                        button.Content = gameBoard.Tiles[i, j].AdjacentMineCount;
                    }
                }
            }
        }

        private bool CheckWinCondition()
        {
            int row = gameBoard.Tiles.GetLength(0);
            int col = gameBoard.Tiles.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (!gameBoard.Tiles[i, j].IsRevealed && !gameBoard.Tiles[i, j].HasMine)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            GameOver();
            gameBoard.ResetBoard();
            grid.Children.Clear();
            AddButtons(gameBoard.Tiles.GetLength(0), gameBoard.Tiles.GetLength(1));
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            LbTimer.Content = elapsedTime.ToString(@"mm\:ss");
        }

        private void GameOver()
        {
            gameTimer.Stop();
            elapsedTime = DateTime.MinValue;
            LbTimer.Content = "00:00";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox != null && gameBoard != null)
            {
                ComboBoxItem typeItem = (ComboBoxItem)comboBox.SelectedItem;
                string s = typeItem.Content.ToString();
                switch (s)
                {
                    case "Beginner": gameBoard.SelectedDifficulty = Difficulty.Beginner; break;
                    case "Intermediate": gameBoard.SelectedDifficulty = Difficulty.Intermediate; break;
                    case "Expert": gameBoard.SelectedDifficulty = Difficulty.Expert; break;
                }

                GameOver();
                ClearGrid();
                int row = gameBoard.Tiles.GetLength(0);
                int col = gameBoard.Tiles.GetLength(1);
                CreateGrid(row, col);
            }
        }

        private void ClearGrid()
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }

        
        private void CustomModalDialog_NewGameClicked(object sender, EventArgs e)
        {
            gameBoard.ResetBoard();
            ClearGrid();
            int row = gameBoard.Tiles.GetLength(0);
            int col = gameBoard.Tiles.GetLength(1);
            CreateGrid(row, col);
            ModalDialog.Visibility = Visibility.Collapsed;
        }

        private void ShowWinDialog()
        {
            ModalDialog.Visibility = Visibility.Visible;
            ModalDialog.messageText.Text = $"Congratulations! You win!\n\tScore: {elapsedTime.ToString(@"mm\:ss")}";
        }

        private void ShowLoseDialog()
        {
            ModalDialog.Visibility = Visibility.Visible;
            ModalDialog.messageText.Text = "You lose!";
        }
        
    }
}
