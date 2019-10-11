using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Reversi
{
    class GameState
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Turn { get; private set; }
        public Point LastMove { get; private set; }
        int[,] cells;

        public GameState(int width, int height)
        {
            Width = width;
            Height = height;

            cells = new int[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    cells[x, y] = -1;

            cells[width / 2, height / 2] = 0;
            cells[width / 2 - 1, height / 2 - 1] = 0;
            cells[width / 2 - 1, height / 2] = 1;
            cells[width / 2, height / 2 - 1] = 1;
        }

        public GameState(GameState original)
        {
            Width = original.Width;
            Height = original.Height;
            Turn = original.Turn;
            LastMove = original.LastMove;
            cells = new int[Width, Height];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    cells[x, y] = original.cells[x, y];
        }

        public bool InBounds(Point p)
        {
            return p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height;
        }

        public int GetCell(int x, int y)
        {
            return cells[x, y];
        }
        public int GetCell(Point p)
        {
            return cells[p.X, p.Y];
        }

        public int CountCells(int player)
        {
            int count = 0;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (cells[x, y] == player)
                        count++;
            return count;
        }

        public bool SetCell(int x, int y)
        {
            Point move = new Point(x, y);
            Point[] captures = GetCaptures(move, Turn);
            if (cells[x, y] == -1 && captures.Length > 0)
            {
                cells[x, y] = Turn;
                foreach (Point p in captures)
                    cells[p.X, p.Y] = Turn;
                LastMove = move;
                NextTurn();
                return true;
            }
            return false;
        }

        public bool IsFinished()
        {
            return GetValidMoves(Turn).Length == 0;
        }

        public double Evaluate(int player)
        {
            if (GetValidMoves(Turn).Length == 0)
                return CountCells(player) > CountCells(1 - player) ? double.PositiveInfinity : double.NegativeInfinity;
            return CountCells(player) / (double)CountCells(1 - player);
        }

        public Point[] GetValidMoves(int player)
        {
            List<Point> moves = new List<Point>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Point p = new Point(x, y);
                    if (GetCell(p) == -1 && GetCaptures(p, player).Length > 0)
                        moves.Add(p);
                }
            }
            return moves.ToArray();
        }

        public Point[] GetCaptures(Point pos, int player)
        {
            List<Point> results = new List<Point>();
            // Ga alle 8 richtingen af
            for(int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    List<Point> ray = new List<Point>();
                    Point p = pos;
                    p.Offset(dx, dy);
                    // Ga door zolang er stenen van de tegenstander zijn en stop ze in een lijst
                    while(InBounds(p) && GetCell(p) == 1-player)
                    {
                        ray.Add(p);
                        p.Offset(dx, dy);
                    }
                    // Als we eindigen op een eigen steen kunnen we deze lijst nemen
                    if (InBounds(p) && GetCell(p) == player)
                        results.AddRange(ray);
                }
            }
            return results.ToArray();
        }


        public void NextTurn()
        {
            Turn = 1 - Turn;
        }

    }
}
