namespace Chess
{
    public class Pawn : Piece
    {
        private readonly Position _forward;

        public Pawn(PlayerColor color) : base(color)
        {
            if (color == PlayerColor.White) _forward = Position.North;
            else _forward = Position.South;
        }

        protected override IEnumerable<Move> GetPossibleMoves(Position startPosition, Board board)
        {
            return GetForwardMoves(startPosition, board).Concat(GetDiagonalMoves(startPosition, board));
        }

        private bool CanMoveTo(Position position, Board board)
        {
            return position.IsValid() && board.IsSquareEmpty(position);
        }
        private bool CanCaptureAt(Position position, Board board) 
        {
            if (!position.IsValid() || board.IsSquareEmpty(position)) return false;
            return board[position].Color != Color;
        }
        private bool CanEnPassant(Position position, Board board)
        {
            if (!position.IsValid() || board.IsSquareEmpty(position)) return false;
            if (board[position].Color == Color) return false;

            if (board[position] == board.LastPieceMoved && board.LastMovePlayed.Flag == MoveFlags.DoublePawnMove)
            {
                return true;
            }

            return false;
        }

        private IEnumerable<Move> GetForwardMoves(Position startPosition, Board board)
        {
            Position oneMovePosition = startPosition + _forward;
            if (CanMoveTo(oneMovePosition, board))
            {
                yield return new Move(startPosition, oneMovePosition);
                Position twoMovePosition = oneMovePosition + _forward;
                if (!HasMoved && CanMoveTo(twoMovePosition, board))
                {
                    yield return new Move(startPosition, twoMovePosition, MoveFlags.DoublePawnMove);
                }
            }
        }        
        private IEnumerable<Move> GetDiagonalMoves(Position startPosition, Board board)
        {
            Position targetForwardWest = startPosition + _forward + Position.West;            
            Position targetForwardEast = startPosition + _forward + Position.East;

            Position targetWest = startPosition + Position.West;
            Position targetEast = startPosition + Position.East;

            if (CanCaptureAt(targetForwardWest, board)) yield return new Move(startPosition, targetForwardWest);
            else if (CanEnPassant(targetWest, board)) yield return new Move(startPosition, targetForwardWest, MoveFlags.EnPassant);

            if (CanCaptureAt(targetForwardEast, board)) yield return new Move(startPosition, targetForwardEast);
            else if (CanEnPassant(targetEast, board)) yield return new Move(startPosition, targetForwardEast, MoveFlags.EnPassant);
        }

        public override bool IsCheckingKing(Position startPosition, Board board)
        {
            return GetDiagonalMoves(startPosition, board).Any(move =>
            {
                Piece piece = board[move.TargetPosition];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
