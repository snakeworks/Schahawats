namespace Chess
{
    public class Queen : Piece
    {
        private static readonly Position[] _moveDirections = new Position[]
        {
            Position.North, Position.South, Position.West, Position.East,
            Position.NorthEast, Position.NorthWest, Position.SouthEast, Position.SouthWest
        };

        public Queen(PlayerColor color) : base(color)
        {
        }

        protected override IEnumerable<Move> GetPossibleMoves(Position startPosition, Board board)
        {
            return GetMovePositionsInDirection(startPosition, board, _moveDirections).Select(target => new Move(startPosition, target));
        }
    }
}
