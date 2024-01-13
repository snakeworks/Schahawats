namespace Chess
{
    public class Rook : Piece
    {
        public Rook(PlayerColor color) : base(color)
        {
        }

        public override bool CanMakeMove(Move move)
        {
            return false;
        }
    }
}
