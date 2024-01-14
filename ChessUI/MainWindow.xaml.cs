using System.Windows;
using System.Windows.Input;

namespace ChessUI
{
    public partial class MainWindow : Window
    {
        private BoardHandler _boardHandler;

        public MainWindow()
        {
            InitializeComponent();
            _boardHandler = new(BoardGrid, PieceGrid, HighlightGrid);
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
    }
}