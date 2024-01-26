using Chess;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessUI
{
    public static class PieceImages
    {
        public static ImageSource GetPieceImage(Piece piece)
        {
            if (piece == null) return null;

            if (piece.Color == PlayerColor.White) return _whiteImageSources[piece.Type];
            else return _blackImageSources[piece.Type];
        }

        private static readonly Dictionary<PieceType, ImageSource> _whiteImageSources = new()
        {
            { PieceType.Pawn, LoadImage("Assets/Board/Pawn_W.png") },
            { PieceType.Knight, LoadImage("Assets/Board/Knight_W.png") },
            { PieceType.Bishop, LoadImage("Assets/Board/Bishop_W.png") },
            { PieceType.Rook, LoadImage("Assets/Board/Rook_W.png") },
            { PieceType.Queen, LoadImage("Assets/Board/Queen_W.png") },
            { PieceType.King, LoadImage("Assets/Board/King_W.png") },
        };        
        private static readonly Dictionary<PieceType, ImageSource> _blackImageSources = new()
        {
            { PieceType.Pawn, LoadImage("Assets/Board/Pawn_B.png") },
            { PieceType.Knight, LoadImage("Assets/Board/Knight_B.png") },
            { PieceType.Bishop, LoadImage("Assets/Board/Bishop_B.png") },
            { PieceType.Rook, LoadImage("Assets/Board/Rook_B.png") },
            { PieceType.Queen, LoadImage("Assets/Board/Queen_B.png") },
            { PieceType.King, LoadImage("Assets/Board/King_B.png") },
        };

        private static ImageSource LoadImage(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }
    }
}
