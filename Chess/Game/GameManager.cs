namespace Chess
{
    public static class GameManager
    {
        public static Board CurrentBoard { get; private set; }
        public static PlayerColor CurrentPlayer { get; private set; }

        public static string LastPgnString { get; private set; }
        public static MatchResult LastMatchResult { get; private set; }

        public static event Action GameStarted;
        public static event Action GameEnded;

        public static void StartGame()
        {
            CurrentBoard = new(Board.FEN_START);
            CurrentPlayer = PlayerColor.White;

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

            CurrentBoard = null;
        }
    }
}
