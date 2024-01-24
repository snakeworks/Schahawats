﻿using System.Diagnostics;

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

        private readonly Dictionary<int, char> _fileSymbols = new(){
            {0, 'a'},
            {1, 'b'},
            {2, 'c'},
            {3, 'd'},
            {4, 'e'},
            {5, 'f'},
            {6, 'g'},
            {7, 'h'}
        };

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

        public void MakeMove(Move move, bool makeAsDummy = false)
        {
            if (!move.IsValid()) return;

            Piece pieceToMove = this[move.StartPosition];
            Piece pieceTarget = this[move.TargetPosition];

            // HACK HACK HACK
            string pgn = "";
            if (!makeAsDummy)
            {
                bool isCheck = false;
                bool isMate = false;
                DummyMoveMade += OnDummyMoveMade;
                void OnDummyMoveMade()
                {
                    DummyMoveMade -= OnDummyMoveMade;
                    isCheck = IsInCheck(pieceToMove.Color.GetOpponent());
                    isMate = IsInCheckmate(pieceToMove.Color.GetOpponent());
                }
                MakeMove(move, true);
                pgn = GetMoveAsPgnString(move, pieceToMove, pieceTarget, isCheck, isMate);
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
        public bool MakeMove(string moveInPgn, PlayerColor pieceColor)
        {
            bool IsFile(char c)
            {
                return c == 'a' || c == 'b' || c == 'c' || c == 'd' || c == 'e' ||
                       c == 'f' || c == 'g' || c == 'h';
            }
            Move GetMove(PieceType type, PlayerColor color, Position targetPos, int overrideRow = -1, int overrideCol = -1)
            {
                Move move;
                for (int i = 0; i < MAX_ROW; i++)
                {
                    for (int j = 0; j < MAX_COLUMN; j++)
                    {
                        if (_pieces[i, j] != null && _pieces[i, j].Type == type && _pieces[i, j].Color == color)
                        {
                            if (overrideRow != -1 && i != overrideRow) continue;
                            if (overrideCol != -1 && j != overrideCol) continue;

                            move = _pieces[i, j].GetLegalMoves(new(i, j), this).GetMoveByTargetPosition(targetPos);
                            if (move != null) return move;
                        }
                    }
                }
                return null;
            }
            Move GetMoveByFlag(PieceType type, PlayerColor color, Position targetPos, MoveFlags flag, int overrideRow = -1, int overrideCol = -1)
            {
                Move move;
                for (int i = 0; i < MAX_ROW; i++)
                {
                    for (int j = 0; j < MAX_COLUMN; j++)
                    {
                        if (_pieces[i, j] != null && _pieces[i, j].Type == type && _pieces[i, j].Color == color)
                        {
                            if (overrideRow != -1 && i != overrideRow) continue;
                            if (overrideCol != -1 && j != overrideCol) continue;

                            move = _pieces[i, j].GetLegalMoves(new(i, j), this).GetMoveByTargetPosition(targetPos, flag);
                            if (move != null) return move;
                        }
                    }
                }
                return null;
            }

            PieceType pieceTypeMoved = PieceType.Pawn;
            PieceType pieceTypePromoted = PieceType.Pawn;

            List<int> rowsFound = new();
            List<int> colsFound = new();

            if (moveInPgn == "O-O")
            {
                pieceTypeMoved = PieceType.King;
                rowsFound.Add(pieceColor == PlayerColor.White ? 7 : 0);
                colsFound.Add(6);
            }
            else if (moveInPgn == "O-O-O")
            {
                pieceTypeMoved = PieceType.King;
                rowsFound.Add(pieceColor == PlayerColor.White ? 7 : 0);
                colsFound.Add(2);
            }
            else
            {
                foreach (var character in moveInPgn)
                {
                    if (Piece.IsPgnPieceSymbol(character))
                    {
                        if (moveInPgn[0] == character)
                        {
                            pieceTypeMoved = Piece.GetPieceTypeBySymbol(character);
                        }
                        else
                        {
                            pieceTypePromoted = Piece.GetPieceTypeBySymbol(character);
                        }
                    }
                    else if (IsFile(character))
                    {
                        var kvp = _fileSymbols.FirstOrDefault(x => x.Value == character);
                        colsFound.Add(kvp.Key);
                    }
                    else if (char.IsDigit(character))
                    {
                        rowsFound.Add(MAX_ROW - int.Parse(character.ToString()));
                    }
                }
            }

            if (rowsFound.Count <= 0 || colsFound.Count <= 0) return false;

            int startRow = rowsFound.Count <= 1 ? -1 : rowsFound[0];
            int startCol = colsFound.Count <= 1 ? -1 : colsFound[0];

            int targetRow = rowsFound[rowsFound.Count - 1];
            int targetCol = colsFound[colsFound.Count - 1];

            Move finalMove;

            if (pieceTypePromoted != PieceType.Pawn)
            {
                finalMove = GetMoveByFlag(pieceTypeMoved, pieceColor, new(targetRow, targetCol), pieceTypePromoted.GetPromotionFlagForPieceType(), startRow, startCol);
            }
            else
            {
                finalMove = GetMove(pieceTypeMoved, pieceColor, new(targetRow, targetCol), startRow, startCol);
            }

            if (finalMove == null) return false;

            MakeMove(finalMove);

            return true;
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
            
        // i hate this :(
        public string GetMoveAsPgnString(Move moveMade, Piece pieceMoved, Piece pieceTargeted, bool isCheck, bool isMate)
        {
            string pgn = "";

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
                char startFile = _fileSymbols[moveMade.StartPosition.Column];
                char targetFile = _fileSymbols[moveMade.TargetPosition.Column];
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

                switch (moveMade.Flag)
                {
                    case MoveFlags.PromoteToQueen:
                        pgn += "=Q";
                        break;
                    case MoveFlags.PromoteToRook:
                        pgn += "=R";
                        break;
                    case MoveFlags.PromoteToBishop:
                        pgn += "=B";
                        break;
                    case MoveFlags.PromoteToKnight:
                        pgn += "=N";
                        break;
                }
            }

            if (isMate) pgn += '#';
            else if (isCheck) pgn += '+';

            return pgn;
        }

        public bool LoadPgn(string pgn)
        {
            IEnumerable<string> GetMovesFromPgn()
            {
                string curParsingMove = string.Empty;
                bool isBracketsClosed = true;
                foreach (var c in pgn)
                {
                    if (c == '[')
                    { 
                        isBracketsClosed = false;
                    }
                    if (c == ']')
                    {
                        isBracketsClosed = true;
                        continue;
                    }

                    if (isBracketsClosed == false) continue;

                    if (char.IsWhiteSpace(c) && !string.IsNullOrEmpty(curParsingMove))
                    {
                        yield return curParsingMove;
                        curParsingMove = string.Empty;
                        continue;
                    }

                    if (string.IsNullOrEmpty(curParsingMove))
                    {
                        if (char.IsDigit(c) || c == '.' || char.IsWhiteSpace(c)) continue;
                        curParsingMove += c;
                    }
                    else
                    {
                        curParsingMove += c;
                    }
                }
            }

            if (string.IsNullOrEmpty(pgn)) return false;

            BoardHistory = new();
            AddHistoryRecord(FEN_START, null, null, null);

            LoadPositionFromFenString(FEN_START);

            var moves = GetMovesFromPgn();
            
            if (moves == null || moves == Enumerable.Empty<Move>()) return false;

            for (int i = 0; i < moves.Count(); i++)
            {
                bool result = MakeMove(moves.ElementAt(i).Trim(), i % 2 == 0 ? PlayerColor.White : PlayerColor.Black);
                if (!result) return false;
            }

            LoadPositionFromFenString(FEN_START);

            return true;
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
