namespace Chess
{
    public abstract class Player : IDisposable
    {
        public abstract bool IsHuman { get; }

        public virtual void Dispose()
        {
        }

        public abstract void OnPlayerTurn(Board board);
    }
}
