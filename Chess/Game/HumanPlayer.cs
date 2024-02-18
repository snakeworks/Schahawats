namespace Chess
{
    public class HumanPlayer : Player
    {
        public override bool IsHuman { get => true; }

        public override void OnPlayerTurn(Board board)
        {
        }
    }
}
