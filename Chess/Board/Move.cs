namespace Chess
{
    public class Move
    {
        public Position StartPosition { get; private set; }
        public Position TargetPosition { get; private set; }

        public Move(Position start, Position target)
        {
            StartPosition = start;
            TargetPosition = target;
        }

        public bool IsValid()
        {
            return StartPosition.IsValid() && TargetPosition.IsValid();
        }
    }
}
