﻿using System.Diagnostics;

namespace Chess
{
    public static class GameManager
    {
        public static Gamemode CurrentGamemode { get; private set; } = Gamemode.None;

        public static Board CurrentBoard { get; private set; }
        public static PlayerColor CurrentPlayer { get; private set; }

        public static event Action GameStarted;
        public static event Action GameEnded;
        public static event Action<MatchResult> MatchEnded;

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
        public static void StopGame()
        {
            CurrentGamemode = Gamemode.None;
            CurrentBoard = null;

            GameEnded?.Invoke();
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
                    MatchEnded?.Invoke(CurrentPlayer.GetOpponent() == PlayerColor.White ? MatchResult.WhiteWins : MatchResult.BlackWins);
                }
                else
                {
                    MatchEnded?.Invoke(MatchResult.Stalemate);
                }

                StopGame();
            }
        }
    }
}
