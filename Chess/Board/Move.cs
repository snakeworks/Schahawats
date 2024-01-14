namespace Chess
{
    public class Move
    {
        public Position StartPosition { get; private set; }
        public Position TargetPosition { get; private set; }
        public MoveFlags Flag { get; private set; }

        public Move(Position start, Position target, MoveFlags flag = MoveFlags.None)
        {
            StartPosition = start;
            TargetPosition = target;
            Flag = flag;
        }

        public bool IsValid()
        {
            return StartPosition.IsValid() && TargetPosition.IsValid();
        }
    }
}
