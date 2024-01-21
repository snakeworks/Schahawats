namespace Chess
{
    public class BoardRecord
    {
        public string Fen { get; private set; }
        public string Pgn { get; private set; }
        public Piece PieceMoved { get; private set; }
        public Move MovePlayed { get; private set; }

        public BoardRecord(string fen, string pgn, Piece pieceMoved, Move move)
        {
            Fen = fen;
            Pgn = pgn;
            PieceMoved = pieceMoved;
            MovePlayed = move;
        }
    }
}
