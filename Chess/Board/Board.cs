using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Chess
{
    public class Board
    {
        public event Action BoardUpdated;

        private readonly Piece[,] _pieces = new Piece[MAX_ROW, MAX_COLUMN];

        public const int MAX_ROW = 8;
        public const int MAX_COLUMN = 8;

        public const string FEN_START = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\r\n";

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
        }

        public bool IsEmpty(Position position)
        {
            return this[position] == null;
        }

        public IEnumerable<Move> GetLegalMovesAtPosition(Position position)
        {
            if (IsEmpty(position)) return Enumerable.Empty<Move>();

            Piece piece = this[position];
            return piece.GetLegalMoves(position, this);
        }
        public IEnumerable<Move> GetAllLegalMoves()
        {
            throw new NotImplementedException();
        }

        public bool TryMakeMove(Move move)
        {
            if (!move.IsValid()) return false;

            Piece pieceToMove = this[move.StartPosition];

            if (pieceToMove == null) return false;
            //if (pieceToMove.GetLegalMoves(move.StartPosition, this).Contains()) return false;

            MakeMove(move);

            return true;
        }

        public void MakeMove(Move move)
        {
            if (!move.IsValid()) return;

            Piece pieceToMove = this[move.StartPosition];

            this[move.StartPosition] = null;
            this[move.TargetPosition] = pieceToMove;

            if (pieceToMove != null) pieceToMove.HasMoved = true;

            BoardUpdated?.Invoke();
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
