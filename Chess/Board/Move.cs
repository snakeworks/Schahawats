namespace Chess
{
    public class Move
    {
        public static readonly Move NullMove = new(new(-1, -1), new(-1, -1));

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

        public override string ToString()
        {
            return $"Start({StartPosition}) Target({TargetPosition}) Flag({Flag})";
        }
    }
}
