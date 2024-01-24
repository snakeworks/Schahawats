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
        private readonly List<Move> _cachedMoves = new();

        private Position _selectedPosition = null;
        
        private readonly UniformGrid _pieceGrid;
        private readonly UniformGrid _highlightGrid;
        private readonly Image _boardImage;
        private readonly Grid _moveHistoryGrid;

        private readonly SolidColorBrush _highlightColor = new(Color.FromArgb(150, 235, 64, 52));
        private readonly SolidColorBrush _previousMoveColor = new(Color.FromArgb(150, 252, 186, 3));

        private readonly Board _previewBoard;

        private PlayerColor _perspective = PlayerColor.White;
        private int _boardViewIndex = 0;

        private List<BoardRecord> _moveHistory;
        private bool _isLoadingPgn = false;

        private Board ActiveBoard
        {
            get
            {
                if (IsViewingCurrentBoard() && GameManager.CurrentBoard != null) return GameManager.CurrentBoard;
                else return _previewBoard;
            }
        }

        public BoardHandler(Image boardImage, UniformGrid pieceGrid, UniformGrid highlightGrid, Grid moveHistoryGrid)
        {
            _boardImage = boardImage;
            _pieceGrid = pieceGrid;
            _highlightGrid = highlightGrid;
            _moveHistoryGrid = moveHistoryGrid;
            _previewBoard = new(Board.FEN_START);
            _previewBoard.BoardUpdated += OnPreviewBoardDraw;
            GameManager.GameStarted += OnGameStarted;
            
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
            
            DrawBoard(_previewBoard);
        }
        public void ResetBoard()
        {
            _moveHistoryGrid.Children.Clear();
            _moveHistoryGrid.RowDefinitions.Clear();
            _moveHistory?.Clear();
            _previewBoard.LoadPositionFromFenString(Board.FEN_START);
        }
        public bool LoadPgn(string pgn)
        {
            ResetBoard();

            _isLoadingPgn = true;

            bool result = _previewBoard.LoadPgn(pgn);

            if (!result)
            {
                _isLoadingPgn = false;
                return false;
            }
            
            _moveHistory = _previewBoard.BoardHistory;
            for (int i = 0; i < _moveHistory.Count; i++)
            {
                if (_moveHistory[i].PieceMoved == null) continue;
                AddMoveHistoryButton(i);
            }

            _isLoadingPgn = false;
            return true;
        }

        private void OnGameStarted()
        {
            ResetBoard();
            GameManager.CurrentBoard.BoardUpdated += OnCurrentBoardDraw;
            GameManager.CurrentBoard.BoardUpdated += OnMoveMade;
            _moveHistory = GameManager.CurrentBoard.BoardHistory;
            OnCurrentBoardDraw();
        }

        private void OnPreviewBoardDraw()
        {
            if (_isLoadingPgn) return;
            DrawBoard(_previewBoard);
        }
        private void OnCurrentBoardDraw()
        {
            _boardViewIndex = _moveHistory.Count - 1;
            DrawBoard(GameManager.CurrentBoard);
        }

        private void DrawBoard(Board board)
        {
            for (int i = 0; i < Board.MAX_ROW; i++)
            {
                for (int j = 0; j < Board.MAX_COLUMN; j++)
                {
                    Position pos = GetPositionBasedOnPerspective(i, j);
                    _pieceImages[pos.Row, pos.Column].Source = PieceImages.GetPieceImage(board[i, j]);
                }
            }

            DeselectCurrentPosition();
            HideAllHighlights();
            ShowLastMoveHighlight();
        }
        private void OnMoveMade()
        {
            _moveHistory = GameManager.CurrentBoard.BoardHistory;
            AddMoveHistoryButton(_moveHistory.Count - 1);
        }
        private void AddMoveHistoryButton(int index)
        {
            int column = index % 2 == 0 ? 2 : 1;

            if (index % 2 != 0)
            {
                RowDefinition definition = new()
                {
                    Height = new GridLength(1, GridUnitType.Auto)
                };
                _moveHistoryGrid.RowDefinitions.Add(definition);
                Label label = new()
                {
                    Content = $"{_moveHistoryGrid.RowDefinitions.Count}.",
                    Foreground = new SolidColorBrush(Colors.White)
                };
                _moveHistoryGrid.Children.Add(label);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, _moveHistoryGrid.RowDefinitions.Count - 1);
            }

            Button button = new()
            {
                Focusable = false,
                Content = _moveHistory[index].Pgn
            };
            button.Click += (s, e) =>
            {
                SetBoardHistoryIndex(index);
            };
            _moveHistoryGrid.Children.Add(button);

            Grid.SetRow(button, _moveHistoryGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(button, column);
        }

        public void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (GameManager.CurrentBoard == null) return;

            Point point = e.GetPosition(_boardImage);
            Position position = GetPositionFromPoint(point);

            if (!position.IsValid() || !IsViewingCurrentBoard()) return;

            if (_selectedPosition == null)
            {
                SelectPosition(position);
            }
            else
            {
                DeselectCurrentPosition();
                TryMakeMove(position);
            }
        }
        void SelectPosition(Position position)
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
        private void DeselectCurrentPosition()
        {
            _selectedPosition = null;
        }
        private void TryMakeMove(Position position)
        {
            Move move = _cachedMoves.ContainsPromotionMoves() ? 
                        _cachedMoves.GetMoveByTargetPosition(position, MoveFlags.PromoteToQueen) :
                        _cachedMoves.GetMoveByTargetPosition(position);

            HideAllHighlights();
            ShowLastMoveHighlight();

            if (move == null) return;

            GameManager.MakeMove(move);
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
            else if (e.Key == Key.F1)
            {
                _perspective = _perspective.GetOpponent();
                DrawBoard(ActiveBoard);
            }
        }

        public void DisplayNextBoardInHistory()
        {
            SetBoardHistoryIndex(_boardViewIndex+1);
        }
        public void DisplayPreviousBoardInHistory()
        {
            SetBoardHistoryIndex(_boardViewIndex-1);
        }
        private void SetBoardHistoryIndex(int index)
        {
            if (!IsMoveHistoryValid()) return;
            _boardViewIndex = index;
            _boardViewIndex = Math.Clamp(_boardViewIndex, 0, _moveHistory.Count - 1);
            _previewBoard.LoadPositionFromFenString(_moveHistory[_boardViewIndex].Fen);
        }
        private bool IsViewingCurrentBoard()
        {
            return _boardViewIndex == _moveHistory.Count - 1;
        }
        private bool IsMoveHistoryValid()
        {
            return _moveHistory != null && _moveHistory.Count > 1;
        }

        private Position GetPositionFromPoint(Point point)
        {
            double squareSize = _boardImage.ActualWidth / Board.MAX_ROW;
            
            int row = (int)(point.Y / squareSize);
            int col = (int)(point.X / squareSize);
            
            return GetPositionBasedOnPerspective(row, col);
        }
        private Position GetPositionBasedOnPerspective(int row, int col)
        {
            return new(_perspective == PlayerColor.White ? row : Board.MAX_ROW - 1 - row,
                       _perspective == PlayerColor.White ? col : Board.MAX_COLUMN - 1 - col);
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
            foreach (var move in _cachedMoves)
            {
                Position pos = GetPositionBasedOnPerspective(move.TargetPosition.Row, move.TargetPosition.Column);
                _highlightedImages[pos.Row, pos.Column].Fill = _highlightColor;
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
            if (!IsMoveHistoryValid()) return;

            Move lastMove = _moveHistory[_boardViewIndex].MovePlayed;

            if (lastMove == null || !lastMove.IsValid()) return;

            Position startPos = GetPositionBasedOnPerspective(lastMove.StartPosition.Row, lastMove.StartPosition.Column);
            Position targetPos = GetPositionBasedOnPerspective(lastMove.TargetPosition.Row, lastMove.TargetPosition.Column);

            _highlightedImages[startPos.Row, startPos.Column].Fill = _previousMoveColor;
            _highlightedImages[targetPos.Row, targetPos.Column].Fill = _previousMoveColor;
        }
    }
}
