namespace Chess
{
    public class Position
    {
        public int Row { get; private set; }
        public int Column { get; private set; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool IsValid()
        {
            if (Row >= Board.MAX_ROW || Row < 0 || Column >= Board.MAX_COLUMN || Row < 0) return false;
            return true;
        }
    }
}
