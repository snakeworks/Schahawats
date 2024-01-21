using Chess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessUI
{
    public partial class MainWindow : Window
    {
        private UIElement _currentTab;
        private Button _currentTabButtonSelected;
        
        private BoardHandler _boardHandler;

        public MainWindow()
        {
            InitializeComponent();
            _boardHandler = new(BoardGrid, PieceGrid, HighlightGrid, MoveHistoryGrid);

            PlayMenu_MoveHistoryMenu.Visibility = Visibility.Collapsed;

            TabPlayButton.Click += (s, e) => OpenTab(TabOptions.Play);
            TabAnalysisButton.Click += (s, e) => OpenTab(TabOptions.Analysis);
            TabPuzzlesButton.Click += (s, e) => OpenTab(TabOptions.Puzzles);
            
            OpenTab(TabOptions.Play);
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
            DisableUIElement(PlayMenu_Buttons);
            EnableUIElement(PlayMenu_MoveHistoryMenu);

            GameManager.StartGame(Gamemode.Normal);
        }

        private void OpenTab(TabOptions option)
        {
            if (_currentTab != null) 
            { 
                DisableUIElement(_currentTab);
                _currentTabButtonSelected.IsEnabled = true;
            }

            switch (option)
            {
                case TabOptions.Play:
                    _currentTabButtonSelected = TabPlayButton;
                    _currentTab = PlayMenu;
                    break;
                case TabOptions.Analysis:
                    _currentTabButtonSelected = TabAnalysisButton;
                    _currentTab = AnalysisMenu;
                    break;
                case TabOptions.Puzzles:
                    _currentTabButtonSelected = TabPuzzlesButton;
                    _currentTab = PuzzlesMenu;
                    break;
            }

            EnableUIElement(_currentTab);
            _currentTabButtonSelected.IsEnabled = false;
        }

        private void EnableUIElement(UIElement element)
        {
            element.Visibility = Visibility.Visible;
            element.IsEnabled = true;
        }        
        private void DisableUIElement(UIElement element)
        {
            element.Visibility = Visibility.Collapsed;
            element.IsEnabled = false;
        }
    }
}