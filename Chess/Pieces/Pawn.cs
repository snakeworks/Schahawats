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

        public override IEnumerable<Move> GetLegalMoves(Position startPosition, Board board)
        {
            return GetForwardMoves(startPosition, board).Concat(GetDiagonalMoves(startPosition, board));
        }

        private bool CanMoveTo(Position position, Board board)
        {
            return position.IsValid() && board.IsEmpty(position);
        }
        private bool CanCaptureAt(Position position, Board board) 
        {
            if (!position.IsValid() || board.IsEmpty(position)) return false;
            return board[position].Color != Color;
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
                    yield return new Move(startPosition, twoMovePosition);
                }
            }
        }        
        private IEnumerable<Move> GetDiagonalMoves(Position startPosition, Board board)
        {
            Position targetWest = startPosition + _forward + Position.West;            
            Position targetEast = startPosition + _forward + Position.East;

            if (CanCaptureAt(targetWest, board)) yield return new Move(startPosition, targetWest);
            if (CanCaptureAt(targetEast, board)) yield return new Move(startPosition, targetEast);
        }
    }
}
