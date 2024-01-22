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

        private BoardHandler _boardHandler;

        public MainWindow()
        {
            InitializeComponent();
            _boardHandler = new(BoardGrid, PieceGrid, HighlightGrid, BoardHistoryGrid);
            GameManager.GameEnded += OnGameEnded;

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
            ExportPgnButton.IsEnabled = false;
            EndGameButton.IsEnabled = true;
            GameManager.StartGame(Gamemode.Normal);
            OpenMenu(BoardHistoryMenu);
        }
        private void ExportPgnButton_Click(object sender, RoutedEventArgs e)
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
            }
            else
            {
                // ...
            }
        }
        private void EndGameButton_Click(object sender, RoutedEventArgs e)
        {
            GameManager.EndGame(MatchResult.ForcefullyEnded);
        }
        private void BackToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            GameManager.EndGame(MatchResult.ForcefullyEnded);
            OpenMenu(MainMenu);
            _boardHandler.ResetBoard();
        }
        private void OnGameEnded()
        {
            EndGameButton.IsEnabled = false;
            ExportPgnButton.IsEnabled = true;
        }

        private void OpenMenu(UIElement menu)
        {
            if (menu == null) return;

            if (_activeMenu != null) _activeMenu.Visibility = Visibility.Collapsed;

            _activeMenu = menu;
            _activeMenu.Visibility = Visibility.Visible;
        }
    }
}