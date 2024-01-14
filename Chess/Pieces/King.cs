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

        protected override IEnumerable<Move> GetPossibleMoves(Position startPosition, Board board)
        {
            return GetMovePositions(startPosition, board).Select(target => new Move(startPosition, target));
        }

        private IEnumerable<Position> GetMovePositions(Position startPosition, Board board)
        {
            foreach (var dir in _moveDirections)
            {
                Position targetPos = startPosition + dir;
                
                if (!targetPos.IsValid()) continue;

                if (board.IsSquareEmpty(targetPos) || board[targetPos].Color != Color) yield return targetPos;
            }
        }

        public override bool IsCheckingKing(Position startPosition, Board board)
        {
            return GetMovePositions(startPosition, board).Any(targetPosition =>
            {
                Piece piece = board[targetPosition];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
