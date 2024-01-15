namespace Chess
{
    public class Position
    {
        public readonly static Position North = new(-1, 0);
        public readonly static Position South = new(1, 0);
        public readonly static Position East = new(0, 1);
        public readonly static Position West = new(0, -1);
        public readonly static Position NorthEast = North + East;
        public readonly static Position NorthWest = North + West;
        public readonly static Position SouthEast = South + East;
        public readonly static Position SouthWest = South + West;

        public int Row { get; private set; }
        public int Column { get; private set; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool IsValid()
        {
            if (Row >= Board.MAX_ROW || Row < 0 || Column >= Board.MAX_COLUMN || Column < 0) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Position position && Row == position.Row && Column == position.Column;
        }
        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }        
        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
        public static Position operator +(Position a, Position b)
        {
            return new(a.Row + b.Row, a.Column + b.Column);
        }
        public static Position operator *(int amount, Position position)
        {
            return new Position(position.Row * amount, position.Column * amount);
        }

        public override string ToString()
        {
            return $"Position: ({Row}, {Column})";
        }
    }
}
