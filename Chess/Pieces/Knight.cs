namespace Chess
{
    public class Knight : Piece
    {
        public Knight(PlayerColor color) : base(color)
        {
        }

        public override IEnumerable<Move> GetLegalMoves(Position startPosition, Board board)
        {
            return GetMovePositions(startPosition, board).Select(target => new Move(startPosition, target));
        }

        private IEnumerable<Position> GetMovePositions(Position startPosition, Board board)
        {
            Position[] verDir = new Position[] { Position.North, Position.South };
            Position[] horDir = new Position[] { Position.West, Position.East };

            foreach (Position vDir in verDir)
            {
                foreach (Position hDir in horDir)
                {
                    Position pos1 = startPosition + 2 * vDir + hDir;
                    Position pos2 = startPosition + 2 * hDir + vDir;

                    if (pos1.IsValid() && (board.IsEmpty(pos1) || board[pos1].Color != Color)) yield return pos1;
                    if (pos2.IsValid() && (board.IsEmpty(pos2) || board[pos2].Color != Color)) yield return pos2;
                }
            }
        }
    }
}
