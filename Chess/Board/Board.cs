using System;
using System.Diagnostics;

namespace Chess
{
    public class Board
    {
        public static Board Instance { get; private set; }

        public event Action BoardUpdated;

        private readonly Piece[,] _pieces = new Piece[MAX_ROW, MAX_COLUMN];

        public const int MAX_ROW = 8;
        public const int MAX_COLUMN = 8;

        private const string FEN_START = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\r\n";

        public Piece this[Position position]
        {
            get { return _pieces[position.Row, position.Column]; }
            set { _pieces[position.Row, position.Column] = value; }
        }

        public Board()
        {
            Instance = this;
            LoadPositionFromFenString(FEN_START);
        }

        public bool TryMakeMove(Move move)
        {
            if (!move.IsValid()) return false;

            Piece pieceToMove = this[move.StartPosition];
            Piece pieceToTarget = this[move.TargetPosition];

            if (pieceToMove == null) return false;
            if (pieceToTarget != null && pieceToTarget.Color == pieceToMove.Color) return false;
            if (!pieceToMove.CanMakeMove(move)) return false;

            MakeMove(move);

            return true;
        }

        public void MakeMove(Move move)
        {
            if (!move.IsValid()) return;

            Piece pieceToMove = this[move.StartPosition];

            this[move.StartPosition] = null;
            this[move.TargetPosition] = pieceToMove;

            pieceToMove.HasMoved = true;
        }

        public void PrintBoard()
        {
            for (int i = 0; i < MAX_ROW; i++)
            {
                string row = "";
                for (int j = 0; j < MAX_COLUMN; j++) 
                {
                    if (_pieces[i, j] == null) row += 'x';
                    else row += _pieces[i, j].Symbol;
                }
                Debug.WriteLine(row);
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
