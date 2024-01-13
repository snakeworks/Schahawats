namespace Chess
{
    public class King : Piece
    {
        private static readonly Position[] _moveDirections = new Position[]
        {
            Position.North, Position.South, Position.West, Position.East,
            Position.NorthEast, Position.NorthWest, Position.SouthEast, Position.SouthWest
        };

        public King(PlayerColor color) : base(color)
        {
        }

        public override IEnumerable<Move> GetLegalMoves(Position startPosition, Board board)
        {
            return GetMovePositions(startPosition, board).Select(target => new Move(startPosition, target));
        }

        private IEnumerable<Position> GetMovePositions(Position startPosition, Board board)
        {
            foreach (var dir in _moveDirections)
            {
                Position targetPos = startPosition + dir;
                
                if (!targetPos.IsValid()) continue;

                if (board.IsEmpty(targetPos) || board[targetPos].Color != Color) yield return targetPos;
            }
        }
    }
}
