namespace Chess
{
    public abstract class Piece
    {
        public PieceType Type
        {
            get
            {
                if (this is Pawn) return PieceType.Pawn;
                else if (this is Knight) return PieceType.Knight;
                else if (this is Bishop) return PieceType.Bishop;
                else if (this is Rook) return PieceType.Rook;
                else if (this is Queen) return PieceType.Queen;
                else if (this is King) return PieceType.King;
                else return PieceType.Pawn;
            }
        }
        public char Symbol
        {
            get
            {
                var symbol = Type switch
                {
                    PieceType.Pawn => 'p',
                    PieceType.Knight => 'n',
                    PieceType.Bishop => 'b',
                    PieceType.Rook => 'r',
                    PieceType.Queen => 'q',
                    PieceType.King => 'k',
                    _ => '\0',
                };
                if (Color == PlayerColor.White) symbol = char.ToUpper(symbol);
                return symbol;
            }
        }
        public PlayerColor Color { get; private set; }
        public bool HasMoved { get; set; } = false;

        public Piece(PlayerColor color)
        {
            Color = color;
        }

        public virtual IEnumerable<Move> GetLegalMoves(Position startPosition, Board board)
        {
            return GetPossibleMoves(startPosition, board).Where(move => IsLegalAfterDummyMove(move, board));
        }

        protected abstract IEnumerable<Move> GetPossibleMoves(Position startPosition, Board board);
        protected bool IsLegalAfterDummyMove(Move move, Board board)
        {
            bool legal = true;
            board.DummyMoveMade += OnMakeDummyMove;
            board.MakeMove(move, true);

            void OnMakeDummyMove()
            {
                board.DummyMoveMade -= OnMakeDummyMove;
                if (board.IsInCheck(Color))
                {
                    legal = false;
                }
            }
            return legal;
        }

        public virtual bool IsCheckingKing(Position startPosition, Board board)
        {
            return GetPossibleMoves(startPosition, board).Any(move =>
            {
                Piece piece = board[move.TargetPosition];
                return piece != null && piece.Type == PieceType.King;
            });
        }

        public static Piece CreatePieceFromSymbol(char symbol)
        {
            Piece piece = null;
            bool isUpper = char.IsUpper(symbol);
            symbol = char.ToLower(symbol);
            switch (symbol)
            {
                case 'p':
                    piece = new Pawn(isUpper == true ? PlayerColor.White : PlayerColor.Black);
                    break;
                case 'n':
                    piece = new Knight(isUpper == true ? PlayerColor.White : PlayerColor.Black);
                    break;
                case 'b':
                    piece = new Bishop(isUpper == true ? PlayerColor.White : PlayerColor.Black);
                    break;
                case 'r':
                    piece = new Rook(isUpper == true ? PlayerColor.White : PlayerColor.Black);
                    break;
                case 'q':
                    piece = new Queen(isUpper == true ? PlayerColor.White : PlayerColor.Black);
                    break;
                case 'k':
                    piece = new King(isUpper == true ? PlayerColor.White : PlayerColor.Black);
                    break;
            }
            return piece;
        }

        protected IEnumerable<Position> GetMovePositionsInDirection(Position startPosition, Board board, Position direction)
        {
            Position curPos = startPosition;
            while (true)
            {
                curPos += direction;

                if (!curPos.IsValid()) yield break;

                if (board.IsSquareEmpty(curPos))
                {
                    yield return curPos;
                    continue;
                }

                Piece piece = board[curPos];

                if (piece.Color != Color)
                {
                    yield return curPos;
                }

                yield break;
            }
        }
        protected IEnumerable<Position> GetMovePositionsInDirection(Position startPosition, Board board, Position[] directions)
        {
            return directions.SelectMany(dir => GetMovePositionsInDirection(startPosition, board, dir));
        }
    }
}
