using Chess;
using System.Windows;

namespace ChessUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BoardVisualizer boardVisualizer = new(PieceGrid);
        }
    }
}