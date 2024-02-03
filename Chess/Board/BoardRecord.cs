namespace Chess
{
    public class BoardRecord
    {
        public string Fen { get; private set; }
        public string MoveInAlgebraicNotation { get; private set; }
        public Piece PieceMoved { get; private set; }
        public Move MovePlayed { get; private set; }

        public BoardRecord(string fen, string an, Piece pieceMoved, Move move)
        {
            Fen = fen;
            MoveInAlgebraicNotation = an;
            PieceMoved = pieceMoved;
            MovePlayed = move;
        }
    }
}
