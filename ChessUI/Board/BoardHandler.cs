using Chess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessUI
{
    public class BoardHandler
    {
        private readonly Image[,] _pieceImages = new Image[Board.MAX_ROW, Board.MAX_COLUMN];
        private readonly Rectangle[,] _highlightedImages = new Rectangle[Board.MAX_ROW, Board.MAX_COLUMN];
        private readonly Dictionary<Position, Move> _cachedSelectedMoves = new();

        private Position _selectedPosition = null;
        
        private readonly UniformGrid _pieceGrid;
        private readonly UniformGrid _highlightGrid;
        private readonly Image _boardImage;

        private readonly SolidColorBrush _highlightColor = new(Color.FromArgb(150, 235, 64, 52));

        public BoardHandler(Image boardImage, UniformGrid pieceGrid, UniformGrid highlightGrid)
        {
            _boardImage = boardImage;
            _pieceGrid = pieceGrid;
            _highlightGrid = highlightGrid;
            InitBoard();
        }

        private void InitBoard()
        {
            GameManager.StartNewGame();
            for (int i = 0; i < Board.MAX_ROW; i++)
            {
                for (int j = 0; j < Board.MAX_COLUMN; j++)
                {
                    Image image = new();
                    _pieceImages[i, j] = image;
                    _pieceGrid.Children.Add(image);

                    Rectangle highlightRect = new();
                    _highlightedImages[i, j] = highlightRect;
                    _highlightGrid.Children.Add(highlightRect);
                }
            }
            GameManager.CurrentBoard.BoardUpdated += DrawBoard;
            DrawBoard();
        }

        private void DrawBoard()
        {
            for (int i = 0; i < Board.MAX_ROW; i++)
            {
                for (int j = 0; j < Board.MAX_COLUMN; j++)
                {
                    _pieceImages[i, j].Source = PieceImages.GetPieceImage(GameManager.CurrentBoard[i, j]);
                }
            }
        }

        public void HandleMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(_boardImage);
            Position position = GetPositionFromPoint(point);

            if (!position.IsValid()) return;

            if (_selectedPosition == null)
            {
                if (!GameManager.CurrentBoard.IsSquareEmpty(position) && GameManager.CurrentBoard[position].Color != GameManager.CurrentPlayer) return;
                
                var moves = GameManager.CurrentBoard.GetLegalMovesAtPosition(position);
                if (moves.Any())
                {
                    _selectedPosition = position;
                    CacheMoves(moves);
                    ShowHighlights();
                }
            }
            else
            {
                _selectedPosition = null;
                HideHighlights();

                if (_cachedSelectedMoves.TryGetValue(position, out var move))
                {
                    GameManager.MakeMove(move);
                }
            }
        }
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                GameManager.CurrentBoard.DisplayPreviousPositionInHistory();
            }
            else if (e.Key == Key.Right)
            {
                GameManager.CurrentBoard.DisplayNextPositionInHistory();
            }
        }

        private Position GetPositionFromPoint(Point point)
        {
            double squareSize = _boardImage.ActualWidth / Board.MAX_ROW;
            int row = (int)(point.Y / squareSize);
            int col = (int)(point.X / squareSize);
            return new(row, col);
        }

        private void CacheMoves(IEnumerable<Move> moves)
        {
            _cachedSelectedMoves.Clear();

            foreach (var move in moves)
            {
                _cachedSelectedMoves[move.TargetPosition] = move;
            }
        }
        private void ShowHighlights()
        {
            foreach (var targetPos in _cachedSelectedMoves.Keys)
            {
                _highlightedImages[targetPos.Row, targetPos.Column].Fill = _highlightColor;
            }
        }
        private void HideHighlights()
        {
            foreach (var targetPos in _cachedSelectedMoves.Keys)
            {
                _highlightedImages[targetPos.Row, targetPos.Column].Fill = Brushes.Transparent;
            }
        }
    }
}
