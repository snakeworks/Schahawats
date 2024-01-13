namespace Chess
{
    public class King : Piece
    {
        public King(PlayerColor color) : base(color)
        {
        }

        public override bool CanMakeMove(Move move)
        {
            return false;
        }
    }
}
