namespace ChessUI
{
    public class ChessGame
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string WhiteName { get; set; }
        public string BlackName { get; set; }
        public string FullPgn { get; set; }
    }
}
