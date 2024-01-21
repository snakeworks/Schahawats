using System.Diagnostics;
using System.Drawing;

namespace Chess
{
    public class Board
    {
        public event Action BoardUpdated;
        public event Action DummyMoveMade;

        private readonly Piece[,] _pieces = new Piece[MAX_ROW, MAX_COLUMN];

        public const int MAX_ROW = 8;
        public const int MAX_COLUMN = 8;

        public const string FEN_START = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\r\n";

        public List<BoardRecord> BoardHistory { get; private set; } = new();

        public Piece LastPieceMoved
        {
            get
            {
                if (BoardHistory.Count <= 0) return null;
                return BoardHistory[BoardHistory.Count - 1].PieceMoved;
            }
        }        
        public Move LastMovePlayed
        {
            get
            {
                if (BoardHistory.Count <= 0) return null;
                return BoardHistory[BoardHistory.Count - 1].MovePlayed;
            }
        }

        public Piece this[int row, int col]
        {
            get { return _pieces[row, col]; }
            private set { _pieces[row, col] = value; }
        }
        public Piece this[Position position]
        {
            get { return _pieces[position.Row, position.Column]; }
            private set { _pieces[position.Row, position.Column] = value; }
        }

        public Board(string fen)
        {
            LoadPositionFromFenString(fen);
            AddHistoryRecord(fen, null, null, null);
        }

        public bool IsSquareEmpty(Position position)
        {
            return this[position] == null;
        }
        public bool AreSquaresEmpty(IEnumerable<Position> positions)
        {
            return positions.All(pos => IsSquareEmpty(pos));
        }

        public IEnumerable<Move> GetLegalMovesAtPosition(Position position)
        {
            if (IsSquareEmpty(position)) return Enumerable.Empty<Move>();

            Piece piece = this[position];
            return piece.GetLegalMoves(position, this);
        }
        public IEnumerable<Move> GetAllLegalMovesFor(PlayerColor color)
        {
            for (int i = 0; i < MAX_ROW; i++)
            {
                for (int j = 0; j < MAX_COLUMN; j++)
                {
                    if (_pieces[i, j] == null || _pieces[i, j].Color != color) continue;

                    foreach (var move in _pieces[i, j].GetLegalMoves(new(i, j), this))
                    {
                        yield return move;
                    }
                }
            }
        }

        // TODO: Rewrite
        public void MakeMove(Move move, bool makeAsDummy = false)
        {
            if (!move.IsValid()) return;

            Piece pieceToMove = this[move.StartPosition];
            Piece pieceTarget = this[move.TargetPosition];

            // Hack
            string pgn = "";
            if (!makeAsDummy)
            { 
                pgn = GetMoveAsPgnString(move, pieceToMove, pieceTarget, IsInCheck(pieceToMove.Color.GetOpponent()), IsInCheckmate(pieceToMove.Color.GetOpponent()));
            }

            switch (move.Flag)
            {
                case MoveFlags.None:
                case MoveFlags.DoublePawnMove:
                    this[move.StartPosition] = null;
                    this[move.TargetPosition] = pieceToMove;
                    break;
                case MoveFlags.EnPassant:
                    this[move.StartPosition] = null;
                    this[move.TargetPosition] = pieceToMove;
                    this[LastMovePlayed.TargetPosition] = null;
                    break;
                case MoveFlags.PromoteToQueen:
                    this[move.StartPosition] = null;
                    this[move.TargetPosition] = new Queen(pieceToMove.Color);
                    break;
                case MoveFlags.PromoteToRook:
                    this[move.StartPosition] = null;
                    this[move.TargetPosition] = new Rook(pieceToMove.Color);
                    break;
                case MoveFlags.PromoteToBishop:
                    this[move.StartPosition] = null;
                    this[move.TargetPosition] = new Bishop(pieceToMove.Color);
                    break;
                case MoveFlags.PromoteToKnight:
                    this[move.StartPosition] = null;
                    this[move.TargetPosition] = new Knight(pieceToMove.Color);
                    break;
                case MoveFlags.CastleKingSide:
                    this[move.StartPosition] = null;
                    this[move.TargetPosition] = pieceToMove;
                    this[move.StartPosition.Row, 5] = this[move.StartPosition.Row, 7];
                    this[move.StartPosition.Row, 7] = null;
                    break;
                case MoveFlags.CastleQueenSide:
                    this[move.StartPosition] = null;
                    this[move.TargetPosition] = pieceToMove;
                    this[move.StartPosition.Row, 3] = this[move.StartPosition.Row, 0];
                    this[move.StartPosition.Row, 0] = null;
                    break;
            }

            if (makeAsDummy)
            {
                DummyMoveMade?.Invoke();
                if (move.Flag == MoveFlags.EnPassant)
                {
                    this[move.StartPosition] = pieceToMove;
                    this[move.TargetPosition] = null;
                    this[LastMovePlayed.TargetPosition] = LastPieceMoved;
                }
                else if (move.Flag == MoveFlags.CastleKingSide)
                {
                    this[move.StartPosition] = pieceToMove;
                    this[move.TargetPosition] = null;
                    this[move.StartPosition.Row, 7] = this[move.StartPosition.Row, 5];
                    this[move.StartPosition.Row, 5] = null;
                }
                else if (move.Flag == MoveFlags.CastleQueenSide)
                {
                    this[move.StartPosition] = pieceToMove;
                    this[move.TargetPosition] = null;
                    this[move.StartPosition.Row, 0] = this[move.StartPosition.Row, 3];
                    this[move.StartPosition.Row, 3] = null;
                }
                else
                {
                    this[move.StartPosition] = pieceToMove;
                    this[move.TargetPosition] = pieceTarget;
                }
            }
            else
            {
                if (pieceToMove != null) pieceToMove.HasMoved = true;

                string fen = GetBoardAsFenString();
                AddHistoryRecord(fen, pgn, pieceToMove, move);

                BoardUpdated?.Invoke();
            }
        }
        public bool IsInCheck(PlayerColor color)
        {
            for (int i = 0; i < MAX_ROW; i++)
            {
                for (int j = 0; j < MAX_COLUMN; j++)
                {
                    if (_pieces[i, j] == null || _pieces[i, j].Color != color.GetOpponent()) continue;
                    if (_pieces[i, j].IsCheckingKing(new(i, j), this)) return true;
                }
            }
            return false;
        }
        public bool IsInCheckmate(PlayerColor color)
        {
            return !GetAllLegalMovesFor(color).Any() && IsInCheck(color);
        }

        public bool IsHistoryRecordValid()
        {
            return BoardHistory.Count > 1;
        }
        private void AddHistoryRecord(string fen, string pgn, Piece pieceToMove, Move movePlayed)
        {
            BoardRecord record = new(fen, pgn, pieceToMove, movePlayed);
            BoardHistory.Add(record);
        }

        public bool IsLastRank(Position position, PlayerColor perspective)
        {
            if (position.Row == 0 && perspective == PlayerColor.White) return true;
            else if (position.Row == MAX_ROW - 1 && perspective == PlayerColor.Black) return true;
            return false;
        }

        public void PrintBoard()
        {
            for (int i = 0; i < MAX_ROW; i++)
            {
                string outline = "-";
                string row = "|";
                for (int j = 0; j < MAX_COLUMN; j++) 
                {
                    outline += "----";
                    if (_pieces[i, j] == null) row += "   |";
                    else row += $" {_pieces[i, j].Symbol} |";
                }
                Debug.WriteLine(outline);
                Debug.WriteLine(row);
                if (i == MAX_ROW - 1) Debug.WriteLine(outline);
            }
        }

        public void LoadPositionFromFenString(string fen)
        {
            ResetBoard();

            string fenBoard = fen.Split(' ')[0];
            int row = 0;
            int column = 0;

            foreach (char symbol in fenBoard)
            {
                if (symbol == '/')
                {
                    row = 0;
                    column++;
                }
                else
                {
                    if (char.IsDigit(symbol))
                    {
                        row += (int)char.GetNumericValue(symbol);
                    }
                    else
                    {
                        Piece piece = Piece.CreatePieceFromSymbol(symbol);
                        _pieces[column, row] = piece;
                        row++;
                    }
                }
            }

            BoardUpdated?.Invoke();
        }
        public string GetBoardAsFenString()
        {
            string fen = "";
            for (int i = 0; i < MAX_ROW; i++)
            {
                int skipCount = 0;
                for (int j = 0; j < MAX_COLUMN; j++)
                {
                    if (_pieces[i, j] != null)
                    {
                        if (skipCount > 0) 
                        { 
                            fen += skipCount.ToString();
                            skipCount = 0;
                        } 
                        fen += _pieces[i, j].Symbol;
                    }
                    else
                    {
                        skipCount++;
                    }
                }
                if (skipCount > 0)
                {
                    fen += skipCount.ToString();
                }
                if (i < MAX_ROW-1) fen += "/";
            }
            return fen;
        }
        public string GetMoveAsPgnString(Move moveMade, Piece pieceMoved, Piece pieceTargeted, bool isCheck, bool isMate)
        {
            string pgn = "";

            Dictionary<int, char> fileSymbols = new()
            {
                {0, 'a'},
                {1, 'b'},
                {2, 'c'},
                {3, 'd'},
                {4, 'e'},
                {5, 'f'},
                {6, 'g'},
                {7, 'h'}
            };

            if (moveMade.Flag == MoveFlags.CastleKingSide)
            {
                pgn += "O-O";
            }
            else if (moveMade.Flag == MoveFlags.CastleQueenSide)
            {
                pgn += "O-O-O";
            }
            else
            {
                char startFile = fileSymbols[moveMade.StartPosition.Column];
                char targetFile = fileSymbols[moveMade.TargetPosition.Column];
                int startRank = (MAX_ROW - 1 - moveMade.StartPosition.Row)+1;
                int targetRank = (MAX_ROW - 1 - moveMade.TargetPosition.Row)+1;

                if (pieceMoved.Type != PieceType.Pawn)
                {
                    pgn += char.ToUpper(pieceMoved.Symbol);
                    for (int i = 0; i < MAX_ROW; i++)
                    {
                        for (int j = 0; j < MAX_COLUMN; j++)
                        {
                            if (_pieces[i, j] == null) continue;
                            if (_pieces[i, j].Color == pieceMoved.Color && _pieces[i, j].Type == pieceMoved.Type && _pieces[i, j] != pieceMoved)
                            {
                                if (_pieces[i, j].GetLegalMoves(new(i, j), this).GetMoveByTargetPosition(moveMade.TargetPosition) != null)
                                {
                                    if (i == moveMade.TargetPosition.Row || j == moveMade.TargetPosition.Column)
                                    {
                                        if (i == moveMade.TargetPosition.Row) pgn += startFile;
                                        if (j == moveMade.TargetPosition.Column) pgn += startRank;
                                    }
                                    else
                                    {
                                        pgn += startFile;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (pieceTargeted != null)
                {
                    pgn += startFile;
                }

                if (pieceTargeted != null) pgn += 'x';

                pgn += $"{targetFile}{targetRank}";
            }

            if (isMate) pgn += '#';
            else if (isCheck) pgn += '+';

            return pgn;
        }
        private void ResetBoard()
        {
            for (int i = 0; i < MAX_ROW; i++)
            {
                for (int j = 0; j < MAX_COLUMN; j++)
                {
                    _pieces[i, j] = null;
                }
            }
        }
    }
}
