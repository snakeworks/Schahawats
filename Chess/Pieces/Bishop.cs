namespace Chess
{
    public class Bishop : Piece
    {
        private static readonly Position[] _moveDirections = new Position[]
        {
            Position.NorthEast, Position.NorthWest, Position.SouthEast, Position.SouthWest
        };

        public Bishop(PlayerColor color) : base(color)
        {
        }

        public override IEnumerable<Move> GetLegalMoves(Position startPosition, Board board)
        {
            return GetMovePositionsInDirection(startPosition, board, _moveDirections).Select(target => new Move(startPosition, target));
        }
    }
}
