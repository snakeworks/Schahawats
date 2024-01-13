namespace Chess
{
    public class Queen : Piece
    {
        public Queen(PlayerColor color) : base(color)
        {
        }

        public override bool CanMakeMove(Move move)
        {
            return false;
        }
    }
}
