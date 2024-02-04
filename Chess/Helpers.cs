namespace Chess
{
    public static class Helpers
    {
        public static PlayerColor GetOpponent(this PlayerColor player)
        {
            return player switch
            {
                PlayerColor.White => PlayerColor.Black,
                PlayerColor.Black => PlayerColor.White,
                _ => PlayerColor.White
            };
        }
        public static bool IsDraw(this MatchResult result)
        {
            return result == MatchResult.Stalemate || result == MatchResult.ThreefoldRepetition ||
                   result == MatchResult.FiftyMoveRule || result == MatchResult.InsufficientMaterial ||
                   result == MatchResult.ForcefullyEnded;
        }
        public static bool IsPromotionFlag(this MoveFlags flag)
        {
            return flag == MoveFlags.PromoteToQueen || flag == MoveFlags.PromoteToRook ||
                   flag == MoveFlags.PromoteToBishop || flag == MoveFlags.PromoteToKnight;
        }
        public static MoveFlags GetPromotionFlagForPieceType(this PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => MoveFlags.None,
                PieceType.Knight => MoveFlags.PromoteToKnight,
                PieceType.Bishop => MoveFlags.PromoteToBishop,
                PieceType.Rook => MoveFlags.PromoteToRook,
                PieceType.Queen => MoveFlags.PromoteToQueen,
                PieceType.King => MoveFlags.None,
                _ => MoveFlags.None,
            };
        }
        public static Move GetMoveByTargetPosition(this IEnumerable<Move> moves, Position targetPosition)
        {
            foreach (var move in moves)
            {
                if (move.TargetPosition == targetPosition)
                {
                    return move;
                }
            }
            return null;
        }
        public static Move GetMoveByTargetPosition(this IEnumerable<Move> moves, Position targetPosition, MoveFlags flag)
        {
            foreach (var move in moves)
            {
                if (move.TargetPosition == targetPosition && move.Flag == flag)
                {
                    return move;
                }
            }
            return null;
        }
        public static bool ContainsPromotionMoves(this IEnumerable<Move> moves)
        {
            foreach (var move in moves)
            {
                if (move.Flag.IsPromotionFlag())
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetBoardHistoryAsPgnExport(this List<BoardRecord> history, string siteTitle = "Schahawats")
        {
            if (history.Count <= 1) return null;

            string date = DateTime.Now.ToString("yyyy.MM.dd");

            MatchResult result;

            if (history[history.Count - 1].MoveInAlgebraicNotation.Trim().EndsWith("#"))
            {
                if (history.Count % 2 == 0) result = MatchResult.WhiteWins;
                else result = MatchResult.BlackWins;
            }
            else
            {
                result = MatchResult.ForcefullyEnded;
            }

            string whiteResult = result == MatchResult.WhiteWins ? "1" : "0";
            whiteResult = result.IsDraw() ? "1/2" : whiteResult;

            string blackResult = result == MatchResult.BlackWins ? "1" : "0";
            blackResult = result.IsDraw() ? "1/2" : blackResult;

            string export = $"[Site \"{siteTitle}\"]\n[Date \"{date}\"]\n[Result \"{whiteResult}-{blackResult}\"]\n\n";

            int realCount = 0;
            int newLineCount = 0;
            int lineCount = 0;
            foreach (var record in history)
            {
                if (string.IsNullOrEmpty(record.MoveInAlgebraicNotation)) continue;
                
                if (realCount % 2 == 0)
                {
                    lineCount++;
                    export += $"{lineCount}.";
                }

                export += $"{record.MoveInAlgebraicNotation} ";

                newLineCount++;
                realCount++;

                if (newLineCount >= 14)
                {
                    export += "\n";
                    newLineCount = 0;
                }
            }

            return export;
        }
    }
}
