using Chess;
using Stockfish.NET;

namespace ChessUI
{
    public class StockfishPlayer : Player
    {
        public override bool IsHuman => false;

        private IStockfish Fish { get; set; }

        public StockfishPlayer(int depth = 2, int level = 20)
        {
            Fish = new Stockfish.NET.Core.Stockfish(@"Engines\Stockfish\stockfish_20090216_x64.exe", depth, new(0, 0, false, 1, level));
        }

        public override async void OnPlayerTurn(Board board)
        {
            await Task.Delay(200);
            string fen = board.GetBoardAsFenString();
            Fish.SetFenPosition(fen);
            string bestMove = Fish.GetBestMove();
            Move move = board.GetMoveFromLongNotation(bestMove);
            GameManager.MakeMove(move);
        }

        public override void Dispose()
        {
            base.Dispose();
            Fish = null;

            // Stockfish process does not exit until garbage collector is called
            // This is a terrible idea
            System.GC.Collect();
        }
    }
}
