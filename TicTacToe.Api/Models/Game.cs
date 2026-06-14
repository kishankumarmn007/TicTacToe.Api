using TicTacToe.Api.Models.Enums;

namespace TicTacToe.Api.Models
{
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Player?[,] Board { get; set; } = new Player?[3, 3];
        public Player CurrentPlayer { get; set; } = Player.X;
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public GameMode Mode { get; set; }
        public Player? Winner { get; set; }
        public List<Move> Moves { get; set; } = new();
        public List<(int, int)> WinningCells { get; set; } = new();
    }
}
