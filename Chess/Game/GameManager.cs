namespace Chess
{
    public static class GameManager
    {

        public static Gamemode CurrentGamemode { get; private set; } = Gamemode.None;

        public static Board CurrentBoard { get; private set; }
        public static PlayerColor CurrentPlayer { get; private set; }

        public static string LastPgnString { get; private set; }
        public static MatchResult LastMatchResult { get; private set; }

        public static event Action GameStarted;
        public static event Action GameEnded;

        public static void StartGame(Gamemode mode)
        {
            CurrentGamemode = mode;

            switch (mode)
            {
                case Gamemode.Normal:
                    CurrentBoard = new(Board.FEN_START);
                    CurrentPlayer = PlayerColor.White;
                    break;
                case Gamemode.Analysis:
                    break;
                case Gamemode.Puzzles:
                    break;
            }

            GameStarted?.Invoke();
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
                    EndGame(CurrentPlayer.GetOpponent() == PlayerColor.White ? MatchResult.WhiteWins : MatchResult.BlackWins);
                }
                else
                {
                    EndGame(MatchResult.Stalemate);
                }
            }
        }

        public static void EndGame(MatchResult result)
        {
            if (CurrentBoard == null) return;

            LastMatchResult = result;
            LastPgnString = CurrentBoard.BoardHistory.GetBoardHistoryAsPgnExport();
            
            GameEnded?.Invoke();

            CurrentGamemode = Gamemode.None;
            CurrentBoard = null;
        }
    }
}
