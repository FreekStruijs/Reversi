using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Reversi
{
    /*
     * Simpele Minimax AI
     * - Ga alle moves af
     */
    static class AI
    {
        public static Point GetBestMove(GameState state)
        {
            GameState best = null;
            Point move = new Point();
            foreach (GameState next in GetPossibleNextStates(state))
            {
                GameState max = MiniMax(next, 4);
                if (best == null || max.Evaluate(state.Turn) > best.Evaluate(state.Turn))
                {
                    best = max;
                    move = next.LastMove;
                }
            }
            return move;
        }

        static GameState MiniMax(GameState state, int depth)
        {
            if (depth == 0) return state;
            GameState[] posibilities = GetPossibleNextStates(state);
            if (posibilities.Length == 0) return state;

            GameState best = null;
            foreach(GameState next in posibilities)
            {
                GameState max = MiniMax(next, depth - 1);
                if (best == null || max.Evaluate(state.Turn) > best.Evaluate(state.Turn))
                    best = max;
            }
            //Console.WriteLine("[D "+depth+"] Point " + best.LastMove + " would score " + best.Evaluate(state.Turn) + " for " + state.Turn);
            return best;
        }

        static GameState[] GetPossibleNextStates(GameState state)
        {
            Point[] moves = state.GetValidMoves(state.Turn);
            List<GameState> result = new List<GameState>();
            foreach(Point p in moves)
            {
                GameState next = new GameState(state);
                next.SetCell(p.X, p.Y);
                result.Add(next);
            }
            return result.ToArray();
        }
    }
}
