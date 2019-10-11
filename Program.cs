using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Reversi
{
    class Program : Form
    {
        const int CELL_SIZE = 40;
        const int FIELD_SIZE = 8;
        Point gridPos = new Point(32, 96);
        GameState game = new GameState(FIELD_SIZE, FIELD_SIZE);
        Brush[] playerBrush = new[] { Brushes.Blue, Brushes.Red };
        string[] playerName = new[] { "Blauw", "Rood" };
        Label[] playerLabel = new[] { new Label(), new Label() };
        Label stateLabel = new Label();
        bool showHelp = false;

        public Program()
        {
            int gridW = game.Width * CELL_SIZE;
            int gridH = game.Height * CELL_SIZE;
            ClientSize = new Size(gridW + 64, gridH + 128);
            Text = "Reversi";

            int btnSize = gridW / 3;
            Button newGameBtn = new Button();
            newGameBtn.Text = "Nieuw spel";
            newGameBtn.Size = new Size(btnSize, 32);
            newGameBtn.Location = new Point(gridPos.X + btnSize * 0, 16);
            newGameBtn.Click += NewGame;
            Controls.Add(newGameBtn);
            Button helpBtn = new Button();
            helpBtn.Text = "Help";
            helpBtn.Size = new Size(btnSize, 32);
            helpBtn.Location = new Point(gridPos.X + btnSize * 1, 16);
            helpBtn.Click += ToggleHelp;
            Controls.Add(helpBtn);
            Button aiBtn = new Button();
            aiBtn.Text = "AI";
            aiBtn.Size = new Size(btnSize, 32);
            aiBtn.Location = new Point(gridPos.X + btnSize * 2, 16);
            aiBtn.Click += AITurn;
            Controls.Add(aiBtn);

            Font font = new Font(DefaultFont.FontFamily, 12);
            stateLabel.Location = new Point(gridPos.X, gridPos.Y + gridH + 4);
            stateLabel.Size = new Size(gridH, 24);
            stateLabel.Font = font;
            Controls.Add(stateLabel);

            playerLabel[0].Location = new Point(gridPos.X + 32, 64);
            playerLabel[0].Font = font;
            playerLabel[1].Location = new Point(ClientSize.Width / 2 + 32, 64);
            playerLabel[1].Font = font;

            Controls.AddRange(playerLabel);

            Paint += OnDrawGame;
            MouseClick += OnMouseClick;
            
            DoubleBuffered = true;

            UpdateLabels();
        }

        private void AITurn(object sender, EventArgs e)
        {
            Point aiMove = AI.GetBestMove(game);
            game.SetCell(aiMove.X, aiMove.Y);
            UpdateLabels();
            Invalidate();
        }

        private void ToggleHelp(object sender, EventArgs e)
        {
            showHelp = !showHelp;
            Invalidate();
        }

        private void NewGame(object sender, EventArgs e)
        {
            game = new GameState(FIELD_SIZE, FIELD_SIZE);
            UpdateLabels();
            Invalidate();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            Point p = new Point((e.X - gridPos.X) / CELL_SIZE, (e.Y - gridPos.Y) / CELL_SIZE);
            if (game.InBounds(p) && game.SetCell(p.X, p.Y))
            {
                UpdateLabels();
                Invalidate();
            }
        }

        void UpdateLabels()
        {
            for (int i = 0; i < playerLabel.Length; i++)
            {
                int count = game.CountCells(i);
                playerLabel[i].Text = count.ToString() + " " + (count == 1 ? "steen" : "stenen");
            }

            if(game.IsFinished())
            {
                stateLabel.Text = playerName[1 - game.Turn] + " heeft gewonnen!";
            }
            else
            {
                stateLabel.Text = playerName[game.Turn] + " is aan de beurt";
            }
        }

        private void OnDrawGame(object sender, PaintEventArgs e)
        {
            Point[] moves = game.GetValidMoves(game.Turn);

            e.Graphics.FillEllipse(playerBrush[0], gridPos.X, 60, 24, 24);
            e.Graphics.FillEllipse(playerBrush[1], ClientSize.Width / 2, 60, 24, 24);
            for (int y = 0; y < game.Height; y++)
            {
                for (int x = 0; x < game.Width; x++)
                {
                    Point p = new Point(x, y);
                    e.Graphics.DrawRectangle(Pens.Black, gridPos.X + x * CELL_SIZE, gridPos.Y + y * CELL_SIZE, CELL_SIZE, CELL_SIZE);
                    int cell = game.GetCell(x, y);
                    if(cell != -1)
                    {
                        e.Graphics.FillEllipse(playerBrush[cell], gridPos.X + x * CELL_SIZE, gridPos.Y + y * CELL_SIZE, CELL_SIZE, CELL_SIZE);
                        if (game.LastMove == p)
                            e.Graphics.DrawEllipse(Pens.White, gridPos.X + x * CELL_SIZE, gridPos.Y + y * CELL_SIZE, CELL_SIZE, CELL_SIZE);
                    }
                    else if(showHelp && moves.Contains(p))
                    {
                        e.Graphics.DrawEllipse(Pens.Black,
                            gridPos.X + x * CELL_SIZE + CELL_SIZE / 4,
                            gridPos.Y + y * CELL_SIZE + CELL_SIZE / 4,
                            CELL_SIZE / 2, CELL_SIZE / 2);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Application.Run(new Program());
        }
    }
}
