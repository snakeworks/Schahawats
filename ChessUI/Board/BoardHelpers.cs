using Chess;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ChessUI
{
    public static class BoardHelpers
    {
        public static ImageSource GetPieceImage(Piece piece)
        {
            if (piece == null) return null;
            return LoadImage($"Assets/Board/{piece.Type}_{(piece.Color == PlayerColor.White ? 'W' : 'B')}.png");
        }
        public static ImageSource GetBoardImage(PlayerColor perspective)
        {
            return LoadImage($"Assets/Board/Board_{(perspective == PlayerColor.White ? 'W' : 'B')}.png");
        }
        private static ImageSource LoadImage(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Relative));
        }
    }
}
