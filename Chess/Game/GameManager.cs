using System.Diagnostics;

namespace Chess
{
    public static class GameManager
    {
        public static Board CurrentBoard { get; private set; }
        public static PlayerColor CurrentPlayer { get; private set; }

        public static event Action<MatchResult> MatchEnded;

        public static void StartNewGame()
        {
            CurrentBoard = new(Board.FEN_START);
            CurrentPlayer = PlayerColor.White;
        }

        public static void MakeMove(Move move)
        {
            if (CurrentBoard[move.StartPosition].Color != CurrentPlayer) return;

            CurrentBoard.MakeMove(move);
            CurrentPlayer = CurrentPlayer.GetOpponent();

            if (!CurrentBoard.GetAllLegalMovesFor(CurrentPlayer).Any())
            {
                if (CurrentBoard.IsInCheck(CurrentPlayer))
                {
                    Debug.WriteLine($"{CurrentPlayer.GetOpponent()} Wins");
                    MatchEnded?.Invoke(CurrentPlayer.GetOpponent() == PlayerColor.White ? MatchResult.WhiteWins : MatchResult.BlackWins);
                }
                else
                {
                    Debug.WriteLine("Stalemate");
                    MatchEnded?.Invoke(MatchResult.Stalemate);
                }
            }
        }
    }
}
