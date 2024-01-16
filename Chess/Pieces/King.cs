namespace Chess
{
    public class King : Piece
    {
        private static readonly Position[] _moveDirections = new Position[]
        {
            Position.North, Position.South, Position.West, Position.East,
            Position.NorthEast, Position.NorthWest, Position.SouthEast, Position.SouthWest
        };

        public King(PlayerColor color) : base(color)
        {
        }

        protected override IEnumerable<Move> GetPossibleMoves(Position startPosition, Board board)
        {
            return GetMovePositions(startPosition, board).Select(target => new Move(startPosition, target));
        }
        public override IEnumerable<Move> GetLegalMoves(Position startPosition, Board board)
        {
            var possibleMoves = GetPossibleMoves(startPosition, board);
            
            var eastPosition = new Position(startPosition.Row, 5);
            var westPosition = new Position(startPosition.Row, 3);

            foreach (var move in possibleMoves)
            {
                if (!IsLegalAfterDummyMove(move, board)) continue;

                if (move.TargetPosition == westPosition)
                {
                    yield return move;
                    Move queenSideCastle = GetQueenSideCastleMove(startPosition, board);
                    if (queenSideCastle != null && IsLegalAfterDummyMove(queenSideCastle, board)) 
                    {
                        yield return queenSideCastle;
                    }
                }
                else if (move.TargetPosition == eastPosition)
                {
                    yield return move;
                    Move kingSideCastle = GetKingSideCastleMove(startPosition, board);
                    if (kingSideCastle != null && IsLegalAfterDummyMove(kingSideCastle, board)) 
                    {
                        yield return kingSideCastle;
                    }
                }
                else
                {
                    yield return move;
                }
            }
        }

        private IEnumerable<Position> GetMovePositions(Position startPosition, Board board)
        {
            foreach (var dir in _moveDirections)
            {
                Position targetPos = startPosition + dir;
                
                if (!targetPos.IsValid()) continue;

                if (board.IsSquareEmpty(targetPos) || board[targetPos].Color != Color) yield return targetPos;
            }
        }
        private bool CanRookCastle(Position position, Board board)
        {
            return !board.IsSquareEmpty(position) && board[position].Type == PieceType.Rook && !board[position].HasMoved;
        }
        private Move GetKingSideCastleMove(Position startPosition, Board board)
        {
            if (HasMoved || board.IsInCheck(Color))
            {
                return null;
            }

            Position rookPositionKingSide = new(startPosition.Row, 7);
            Position[] positionsBetweenRookKingSide = new Position[] { new(startPosition.Row, 5), new(startPosition.Row, 6) };

            if (CanRookCastle(rookPositionKingSide, board) && board.AreSquaresEmpty(positionsBetweenRookKingSide))
            {
                return new Move(startPosition, new(startPosition.Row, 6), MoveFlags.CastleKingSide);
            }
            
            return null;
        }
        private Move GetQueenSideCastleMove(Position startPosition, Board board)
        {
            if (HasMoved || board.IsInCheck(Color))
            {
                return null;
            }

            Position rookPositionQueenSide = new(startPosition.Row, 0);
            Position[] positionsBetweenRookQueenSide = new Position[] { new(startPosition.Row, 3), new(startPosition.Row, 2), new(startPosition.Row, 1) };

            if (CanRookCastle(rookPositionQueenSide, board) && board.AreSquaresEmpty(positionsBetweenRookQueenSide))
            {
                return new Move(startPosition, new(startPosition.Row, 2), MoveFlags.CastleQueenSide);
            }
            
            return null;
        }

        public override bool IsCheckingKing(Position startPosition, Board board)
        {
            return GetMovePositions(startPosition, board).Any(targetPosition =>
            {
                Piece piece = board[targetPosition];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
