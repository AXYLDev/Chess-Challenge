using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // A simple bot that can spot mate in one, and always captures the most valuable piece it can.
    // Plays randomly otherwise.
    public class EvilBot : IChessBot
    {
        uint s_maxDepth = 3;
        float[] s_pieceValues = { 0, 1, 3, 3, 5, 9 };
        bool m_isWhite;

        public Move Think(Board board, Timer timer)
        {
            m_isWhite = board.IsWhiteToMove;
            float max = -1024f;
            Move[] moves = board.GetLegalMoves();
            Move bestMove = moves[0];
            for (uint i = 0; i < moves.Length; i++)
            {
                float v = DeltaValue(moves[i], board, 0);
                if (v > max) { max = v; bestMove = moves[i]; }
            }
            return bestMove;
        }

        float DeltaValue(Move move, Board board, uint depth)
        {
            float value = (m_isWhite == board.IsWhiteToMove ? 1 : -1) * s_pieceValues[(int)board.GetPiece(move.TargetSquare).PieceType];
            if (depth >= s_maxDepth) return value;
            board.MakeMove(move);
            Move[] moves = board.GetLegalMoves();
            for (uint i = 0; i < moves.Length; i++)
                value += DeltaValue(moves[i], board, depth + 1) / moves.Length;
            board.UndoMove(move);
            return value;
        }
    }
}