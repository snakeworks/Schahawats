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
            return LoadImage($"Assets/Board/{piece.Type}_{(piece.Color == PlayerColor.White ? 'W' : 'B')}.png");
        }

        private static ImageSource LoadImage(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }
    }
}
