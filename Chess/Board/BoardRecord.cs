namespace Chess
{
    public class BoardRecord
    {
        public string Fen { get; private set; }
        public Piece PieceMoved { get; private set; }
        public Move MovePlayed { get; private set; }

        public BoardRecord(string fen, Piece piece, Move move)
        {
            Fen = fen;
            PieceMoved = piece;
            MovePlayed = move;
        }
    }
}
