using Chess;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ChessUI
{
    public class BoardVisualizer
    {
        private readonly Image[,] _pieceImages = new Image[Board.MAX_ROW, Board.MAX_COLUMN];
        private readonly UniformGrid _pieceGrid;

        private Board _currentBoard;

        public BoardVisualizer(UniformGrid grid)
        {
            _pieceGrid = grid;
            InitBoard();

            _currentBoard.MakeMove(new Move(new(0, 3), new(3, 2)));
        }

        private void InitBoard()
        {
            _currentBoard = new(Board.FEN_START);
            for (int i = 0; i < Board.MAX_ROW; i++)
            {
                for (int j = 0; j < Board.MAX_COLUMN; j++)
                {
                    Image image = new();
                    _pieceImages[i, j] = image;
                    _pieceGrid.Children.Add(image);
                }
            }
            _currentBoard.BoardUpdated += DrawBoard;
            DrawBoard();
        }

        private void DrawBoard()
        {
            for (int i = 0; i < Board.MAX_ROW; i++)
            {
                for (int j = 0; j < Board.MAX_COLUMN; j++)
                {
                    _pieceImages[i, j].Source = PieceImages.GetPieceImage(_currentBoard[i, j]);
                }
            }
        }
    }
}
