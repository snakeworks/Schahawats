using Chess;
using System.Diagnostics;
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
        private readonly List<Move> _cachedMoves = new();

        private Position _selectedPosition = null;
        
        private readonly UniformGrid _pieceGrid;
        private readonly UniformGrid _highlightGrid;
        private readonly Image _boardImage;

        private readonly SolidColorBrush _highlightColor = new(Color.FromArgb(150, 235, 64, 52));
        private readonly SolidColorBrush _previousMoveColor = new(Color.FromArgb(150, 252, 186, 3));

        private readonly Board _previewBoard;

        private int _boardViewIndex = 0;

        public BoardHandler(Image boardImage, UniformGrid pieceGrid, UniformGrid highlightGrid)
        {
            _boardImage = boardImage;
            _pieceGrid = pieceGrid;
            _highlightGrid = highlightGrid;
            _previewBoard = new(Board.FEN_START);
            _previewBoard.BoardUpdated += () => DrawBoard(_previewBoard);
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
            GameManager.CurrentBoard.BoardUpdated += () => DrawBoard(GameManager.CurrentBoard);
            DrawBoard(GameManager.CurrentBoard);
        }

        private void DrawBoard(Board board)
        {
            if (board == GameManager.CurrentBoard) _boardViewIndex = GameManager.CurrentBoard.BoardHistory.Count - 1;

            for (int i = 0; i < Board.MAX_ROW; i++)
            {
                for (int j = 0; j < Board.MAX_COLUMN; j++)
                {
                    _pieceImages[i, j].Source = PieceImages.GetPieceImage(board[i, j]);
                }
            }

            HideAllHighlights();
            ShowLastMoveHighlight();
        }

        public void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(_boardImage);
            Position position = GetPositionFromPoint(point);

            if (!position.IsValid() || !IsViewingCurrentBoard()) return;

            if (_selectedPosition == null)
            {
                if (!GameManager.CurrentBoard.IsSquareEmpty(position) && GameManager.CurrentBoard[position].Color != GameManager.CurrentPlayer) return;
                
                var moves = GameManager.CurrentBoard.GetLegalMovesAtPosition(position);
                if (moves.Any())
                {
                    _selectedPosition = position;
                    CacheMoves(moves);
                    ShowMoveHighlights();
                }
            }
            else
            {
                _selectedPosition = null;

                Move move = _cachedMoves.GetMoveByTargetPosition(position);

                HideAllHighlights();
                ShowLastMoveHighlight();

                if (move == Move.NullMove) return;

                if (_cachedMoves.ContainsPromotionMoves())
                {
                    GameManager.MakeMove(_cachedMoves.GetMoveByFlag(MoveFlags.PromoteToQueen));
                }
                else
                {
                    GameManager.MakeMove(move);
                }
            }
        }
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                DisplayPreviousBoardInHistory();
            }
            else if (e.Key == Key.Right)
            {
                DisplayNextBoardInHistory();
            }
        }

        private void DisplayNextBoardInHistory()
        {
            if (GameManager.CurrentBoard.BoardHistory.Count <= 0) return;
            _boardViewIndex++;
            _boardViewIndex = Math.Clamp(_boardViewIndex, 0, GameManager.CurrentBoard.BoardHistory.Count - 1);
            _previewBoard.LoadPositionFromFenString(GameManager.CurrentBoard.BoardHistory[_boardViewIndex].Fen);
        }
        private void DisplayPreviousBoardInHistory()
        {
            if (GameManager.CurrentBoard.BoardHistory.Count <= 0) return;
            _boardViewIndex--;
            _boardViewIndex = Math.Clamp(_boardViewIndex, 0, GameManager.CurrentBoard.BoardHistory.Count - 1);
            _previewBoard.LoadPositionFromFenString(GameManager.CurrentBoard.BoardHistory[_boardViewIndex].Fen);
        }
        private bool IsViewingCurrentBoard()
        {
            return _boardViewIndex == GameManager.CurrentBoard.BoardHistory.Count - 1;
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
            _cachedMoves.Clear();

            foreach (var move in moves)
            {
                _cachedMoves.Add(move);
            }
        }
        private void ShowMoveHighlights()
        {
            foreach (var targetPos in _cachedMoves)
            {
                _highlightedImages[targetPos.TargetPosition.Row, targetPos.TargetPosition.Column].Fill = _highlightColor;
            }
        }
        private void HideAllHighlights()
        {
            for (int i = 0; i < Board.MAX_ROW; i++)
            {
                for (int j = 0; j < Board.MAX_COLUMN; j++)
                {
                    _highlightedImages[i, j].Fill = Brushes.Transparent;
                }
            }
        }
        private void ShowLastMoveHighlight()
        {
            if (GameManager.CurrentBoard.BoardHistory.Count <= 0) return;

            Move lastMove = GameManager.CurrentBoard.BoardHistory[_boardViewIndex].MovePlayed;
            if (lastMove != null)
            {
                _highlightedImages[lastMove.StartPosition.Row, lastMove.StartPosition.Column].Fill = _previousMoveColor;
                _highlightedImages[lastMove.TargetPosition.Row, lastMove.TargetPosition.Column].Fill = _previousMoveColor;
            }
        }
    }
}
