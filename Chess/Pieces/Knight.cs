namespace Chess
{
    public class Knight : Piece
    {
        public Knight(PlayerColor color) : base(color)
        {
        }

        public override bool CanMakeMove(Move move)
        {
            return false;
        }
    }
}
