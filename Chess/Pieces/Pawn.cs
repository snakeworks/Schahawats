namespace Chess
{
    public class Pawn : Piece
    {
        public Pawn(PlayerColor color) : base(color)
        {
        }

        public override bool CanMakeMove(Move move)
        {
            return false;
        }
    }
}
