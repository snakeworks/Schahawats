using Chess;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessUI
{
    public partial class MainWindow : Window
    {
        private UIElement _activeMenu;

        private readonly BoardHandler _boardHandler;

        public MainWindow()
        {
            InitializeComponent();
            _boardHandler = new(BoardGrid, PieceGrid, HighlightGrid, BoardHistoryGrid);
            GameManager.GameEnded += OnGameEnded;

            CreateAllButtonsForGamesFromDatabase();

            OpenMenu(MainMenu);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _boardHandler.HandleMouseDown(sender, e);
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _boardHandler.HandleKeyDown(sender, e);
        }

        private void NewGameButton_Clicked(object sender, RoutedEventArgs e)
        {
            SavePgnButton.IsEnabled = false;
            EndGameButton.IsEnabled = true;

            EndGameButton.Visibility = Visibility.Visible;
            SavePgnButton.Visibility = Visibility.Visible;

            GameManager.StartGame();

            SetGameTitle("LOCAL GAME");

            OpenMenu(BoardHistoryMenu);
        }
        private void SavePgnButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                DefaultExt = ".txt",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string path = saveFileDialog.FileName;
                File.WriteAllText(path, GameManager.LastPgnString);
                MessageBox.Show($"Successfully saved PGN to '{path}'", "Save PGN", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void LoadPgnButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                DefaultExt = ".txt",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string path = openFileDialog.FileName;
                using StreamReader reader = new(path);   
                string pgn = reader.ReadToEnd();

                OpenGameBoardWithPgn(pgn, "PGN GAME");
            }
        }
        private void OpenGameBoardWithPgn(string pgn, string title = "")
        {
            bool pgnResult = _boardHandler.LoadPgn(pgn);
            SetGameTitle(title);

            if (pgnResult == true)
            {
                EndGameButton.Visibility = Visibility.Collapsed;
                SavePgnButton.Visibility = Visibility.Collapsed;
                OpenMenu(BoardHistoryMenu);
            }
            else
            {
                MessageBox.Show("The PGN of the game is invalid.", "Load PGN", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SetGameTitle(string title)
        {
            GameTitleTextBlock.Text = title;
        }

        private void EndGameButton_Click(object sender, RoutedEventArgs e)
        {
            GameManager.EndGame(MatchResult.ForcefullyEnded);
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameManager.CurrentBoard != null)
            {
                var result = MessageBox.Show("Returning to the main menu will cancel the current game. Continue?", "Cancel Game", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;
            }

            GameManager.EndGame(MatchResult.ForcefullyEnded);
            OpenMenu(MainMenu);
            _boardHandler.ResetBoard();
        }
        private void OnGameEnded()
        {
            EndGameButton.IsEnabled = false;
            SavePgnButton.IsEnabled = true;
        }

        private void OpenMenu(UIElement menu)
        {
            if (menu == null) return;

            if (menu == MainMenu)
            {
                BackButton.Visibility = Visibility.Collapsed;
                VersionTextBlock.Visibility = Visibility.Visible;
            }
            else 
            {
                BackButton.Visibility = Visibility.Visible;
                VersionTextBlock.Visibility = Visibility.Collapsed;
            } 

            if (_activeMenu != null) _activeMenu.Visibility = Visibility.Collapsed;

            _activeMenu = menu;
            _activeMenu.Visibility = Visibility.Visible;
        }

        private void CreateAllButtonsForGamesFromDatabase()
        {
            GameExplorerPanel.Children.Clear();
            var allGames = DatabaseHandler.GetAllGames();

            if (allGames == null || !allGames.Any()) return;

            foreach (var game in DatabaseHandler.GetAllGames())
            {
                string title = $"{game.WhiteName.Trim() } vs {game.BlackName.Trim()}";
                Button button = new()
                {
                    Content = $"{title}\n({game.Date})",
                    Height = 42,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                };
                button.Click += (s, e) =>
                {
                    OpenGameBoardWithPgn(game.FullPgn, title);
                };
                GameExplorerPanel.Children.Add(button);
            }
        }

        private void PreviewBoardNext_Click(object sender, RoutedEventArgs e)
        {
            _boardHandler.DisplayNextBoardInHistory();
        }
        private void PreviewBoardPrevious_Click(object sender, RoutedEventArgs e)
        {
            _boardHandler.DisplayPreviousBoardInHistory();
        }
        private void FlipButton_Click(object sender, RoutedEventArgs e)
        {
            _boardHandler.FlipPerspective();
        }

        private void ExploreGamesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenMenu(GameExplorerMenu);
        }
    }
}