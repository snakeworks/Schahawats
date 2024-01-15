namespace Chess
{
    public static class Helpers
    {
        public static PlayerColor GetOpponent(this PlayerColor player)
        {
            return player switch
            {
                PlayerColor.White => PlayerColor.Black,
                PlayerColor.Black => PlayerColor.White,
                _ => PlayerColor.None,
            };
        }
        public static Move GetMoveByTargetPosition(this IEnumerable<Move> moves, Position targetPosition)
        {
            foreach (var move in moves)
            {
                if (move.TargetPosition == targetPosition)
                {
                    return move;
                }
            }
            return null;
        }
        public static Move GetMoveByFlag(this IEnumerable<Move> moves, MoveFlags flag)
        {
            foreach (var move in moves)
            {
                if (move.Flag == flag)
                {
                    return move;
                }
            }
            return null;
        }
        public static bool ContainsPromotionMoves(this IEnumerable<Move> moves)
        {
            foreach (var move in moves)
            {
                if (move.Flag == MoveFlags.PromoteToQueen || move.Flag == MoveFlags.PromoteToRook
                    || move.Flag == MoveFlags.PromoteToBishop || move.Flag == MoveFlags.PromoteToKnight)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
