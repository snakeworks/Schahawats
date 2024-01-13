﻿using Chess;
using System.Windows;

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

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                _boardHandler.HandleMouseDown(sender, e);
        }
    }
}