using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Enums;

namespace TicTacToe.Api.Service
{
    public class GameService
    {
        private readonly Dictionary<Guid, Game> games = new();
        private readonly Scoreboard score;

        public GameService(Scoreboard score)
        {
            this.score = score;
        }

        public Game Create(GameMode mode)
        {
            var g = new Game { Mode = mode };
            games[g.Id] = g;
            return g;
        }

        public Game Get(Guid id) => games[id];

        public Game Move(Guid id, int row, int col)
        {
            var g = games[id];

            if (g.Status != GameStatus.InProgress)
                throw new Exception("Game completed");

            if (row < 0 || col < 0 || row > 2 || col > 2)
                throw new Exception("Invalid position");

            if (g.Board[row, col] != null)
                throw new Exception("Cell occupied");

            g.Board[row, col] = g.CurrentPlayer;

            g.Moves.Add(new Move
            {
                MoveNumber = g.Moves.Count + 1,
                Player = g.CurrentPlayer,
                Row = row,
                Column = col
            });

            CheckResult(g);

            if (g.Status == GameStatus.InProgress)
            {
                g.CurrentPlayer = g.CurrentPlayer == Player.X ? Player.O : Player.X;

                if (g.Mode == GameMode.Computer && g.CurrentPlayer == Player.O)
                {
                    var aiMove = GetComputerMove(g);
                    Move(id, aiMove.Item1, aiMove.Item2);
                }
            }

            return g;
        }

        public Game Undo(Guid id)
        {
            var g = games[id];

            if (g.Status != GameStatus.InProgress)
                throw new Exception("Undo disabled after completion");

            int steps = g.Mode == GameMode.Computer ? 2 : 1;

            for (int i = 0; i < steps && g.Moves.Count > 0; i++)
            {
                var last = g.Moves.Last();
                g.Board[last.Row, last.Column] = null;
                g.Moves.RemoveAt(g.Moves.Count - 1);
            }

            g.CurrentPlayer = g.Moves.LastOrDefault()?.Player == Player.X ? Player.O : Player.X;

            g.Status = GameStatus.InProgress;
            g.Winner = null;
            g.WinningCells.Clear();

            return g;
        }

        public Game Reset(Guid id)
        {
            var g = games[id];

            g.Board = new Player?[3, 3];
            g.Moves.Clear();
            g.CurrentPlayer = Player.X;
            g.Status = GameStatus.InProgress;
            g.Winner = null;
            g.WinningCells.Clear();

            return g;
        }

        public Game Skip(Guid id)
        {
            var g = games[id];

            if (g.Status != GameStatus.InProgress)
                throw new Exception("Game completed");

            g.CurrentPlayer = g.CurrentPlayer == Player.X ? Player.O : Player.X;

            if (g.Mode == GameMode.Computer && g.CurrentPlayer == Player.O)
            {
                var aiMove = GetComputerMove(g);
                Move(id, aiMove.Item1, aiMove.Item2);
            }

            return g;
        }

        public Scoreboard GetScore() => score;

        public void ResetScore()
        {
            score.XWins = score.OWins = score.Draws = 0;
        }

        private void CheckResult(Game g)
        {
            var lines = new List<(int, int)[]>
        {
            new[]{(0,0),(0,1),(0,2)},
            new[]{(1,0),(1,1),(1,2)},
            new[]{(2,0),(2,1),(2,2)},
            new[]{(0,0),(1,0),(2,0)},
            new[]{(0,1),(1,1),(2,1)},
            new[]{(0,2),(1,2),(2,2)},
            new[]{(0,0),(1,1),(2,2)},
            new[]{(0,2),(1,1),(2,0)}
        };

            foreach (var line in lines)
            {
                var vals = line.Select(c => g.Board[c.Item1, c.Item2]).ToList();

                if (vals.All(v => v != null) && vals.Distinct().Count() == 1)
                {
                    g.Status = GameStatus.Won;
                    g.Winner = vals.First();
                    g.WinningCells = line.ToList();

                    if (g.Winner == Player.X) score.XWins++;
                    else score.OWins++;

                    return;
                }
            }

            if (g.Moves.Count == 9)
            {
                g.Status = GameStatus.Draw;
                score.Draws++;
            }
        }

        private (int, int) GetComputerMove(Game g)
        {
            // WIN
            var win = FindMove(g, Player.O);
            if (win != null) return win.Value;

            // BLOCK
            var block = FindMove(g, Player.X);
            if (block != null) return block.Value;

            // CENTER
            if (g.Board[1, 1] == null) return (1, 1);

            // CORNERS
            var corners = new[] { (0, 0), (0, 2), (2, 0), (2, 2) };
            foreach (var c in corners)
                if (g.Board[c.Item1, c.Item2] == null)
                    return c;

            // ANY
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (g.Board[i, j] == null)
                        return (i, j);

            throw new Exception();
        }

        private (int, int)? FindMove(Game g, Player p)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (g.Board[i, j] == null)
                    {
                        g.Board[i, j] = p;
                        if (IsWin(g, p))
                        {
                            g.Board[i, j] = null;
                            return (i, j);
                        }
                        g.Board[i, j] = null;
                    }
                }
            return null;
        }

        private bool IsWin(Game g, Player p)
        {
            var b = g.Board;
            for (int i = 0; i < 3; i++)
                if (b[i, 0] == p && b[i, 1] == p && b[i, 2] == p) return true;

            for (int j = 0; j < 3; j++)
                if (b[0, j] == p && b[1, j] == p && b[2, j] == p) return true;

            if (b[0, 0] == p && b[1, 1] == p && b[2, 2] == p) return true;
            if (b[0, 2] == p && b[1, 1] == p && b[2, 0] == p) return true;

            return false;
        }
    }
}
