namespace Chess
{
    public class Bishop : Piece
    {
        public Bishop(PlayerColor color) : base(color)
        {
        }

        public override bool CanMakeMove(Move move)
        {
            return false;
        }
    }
}
