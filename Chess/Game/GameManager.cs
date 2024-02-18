namespace Chess
{
    public static class GameManager
    {
        public static Board CurrentBoard { get; private set; }
        public static PlayerColor CurrentPlayerColor { get; private set; }
        public static Player CurrentPlayer
        {
            get
            {
                if (CurrentPlayerColor == PlayerColor.White) return PlayerWhite;
                else return PlayerBlack;
            }
        }

        public static Player PlayerWhite { get; private set; }
        public static Player PlayerBlack { get; private set; }

        public static string LastPgnString { get; private set; }
        public static MatchResult LastMatchResult { get; private set; }

        public static event Action GameStarted;
        public static event Action GameEnded;

        public static void StartGame(Player playerWhite = null, Player playerBlack = null)
        {
            PlayerWhite = playerWhite ?? new HumanPlayer();
            PlayerBlack = playerBlack ?? new HumanPlayer();

            CurrentBoard = new(Board.FEN_START);
            CurrentPlayerColor = PlayerColor.White;

            GameStarted?.Invoke();

            OnMoveMade();
        }

        public static void MakeMove(Move move)
        {
            if (CurrentBoard[move.StartPosition].Color != CurrentPlayerColor) return;

            CurrentBoard.MakeMove(move);
            CurrentPlayerColor = CurrentPlayerColor.GetOpponent();

            if (!CurrentBoard.GetAllLegalMovesFor(CurrentPlayerColor).Any())
            {
                if (CurrentBoard.IsInCheck(CurrentPlayerColor))
                {
                    EndGame(CurrentPlayerColor.GetOpponent() == PlayerColor.White ? MatchResult.WhiteWins : MatchResult.BlackWins);
                }
                else
                {
                    EndGame(MatchResult.Stalemate);
                }
            }
            
            OnMoveMade();
        }
        private static void OnMoveMade()
        {
            if (PlayerWhite == null || PlayerBlack == null) return;

            if (CurrentPlayerColor == PlayerColor.White) PlayerWhite.OnPlayerTurn(CurrentBoard);
            else PlayerBlack.OnPlayerTurn(CurrentBoard);
        }

        public static void EndGame(MatchResult result)
        {
            if (CurrentBoard == null) return;

            PlayerWhite.Dispose();
            PlayerBlack.Dispose();

            PlayerWhite = null;
            PlayerBlack = null;

            LastMatchResult = result;
            LastPgnString = CurrentBoard.BoardHistory.GetBoardHistoryAsPgnExport();
            
            GameEnded?.Invoke();

            CurrentBoard = null;
        }
    }
}
