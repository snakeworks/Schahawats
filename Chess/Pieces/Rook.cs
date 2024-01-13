namespace Chess
{
    public class Rook : Piece
    {
        private static readonly Position[] _moveDirections = new Position[]
        {
            Position.North, Position.South, Position.West, Position.East
        };

        public Rook(PlayerColor color) : base(color)
        {
        }

        public override IEnumerable<Move> GetLegalMoves(Position startPosition, Board board)
        {
            return GetMovePositionsInDirection(startPosition, board, _moveDirections).Select(target => new Move(startPosition, target));
        }
    }
}
