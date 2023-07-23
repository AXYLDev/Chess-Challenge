using ChessChallenge.API;

public class MyBot : IChessBot
{
    float m_timePerTurn = 250;
    float[] s_pieceValues = { 0, 1, 3, 3, 5, 9 };
    uint m_maxDepth = 3;
    bool m_isWhite;

    public Move Think(Board board, Timer timer)
    {
        m_isWhite = board.IsWhiteToMove;
        float max = -1024f;
        Move[] moves = board.GetLegalMoves();
        Move bestMove = moves[0];
        for (uint i = 0; i < moves.Length; i++) { 
            float v = DeltaValue(moves[i], board, 0);
            if (v > max) { max = v; bestMove = moves[i]; }
        }
        // Find constant for (time = c * 2 ^ depth)
        //double c = timer.MillisecondsElapsedThisTurn / System.Math.Pow(moves.Length, m_maxDepth);
        //int newDepth = (int)System.Math.Log(m_timePerTurn / c, moves.Length);
        //m_maxDepth = (uint)System.Math.Clamp(System.Math.Clamp((int)newDepth, m_maxDepth - 1, m_maxDepth + 1), 2, 8);
        return bestMove;
    }

    float DeltaValue(Move move, Board board, uint depth)
    {
        float value = s_pieceValues[(int)board.GetPiece(move.TargetSquare).PieceType];
        board.MakeMove(move);
        if (board.IsInCheckmate()) value += 20;
        if (m_isWhite == board.IsWhiteToMove) value = -value;
        if (depth >= m_maxDepth || board.IsInCheckmate()) {
            board.UndoMove(move);
            return value;
        }
        Move[] moves = board.GetLegalMoves();
        for (uint i = 0; i < moves.Length; i++)
            value += DeltaValue(moves[i], board, depth + 1) / moves.Length;
        board.UndoMove(move);
        return value;
    }
}
