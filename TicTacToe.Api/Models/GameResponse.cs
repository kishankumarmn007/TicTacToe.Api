using TicTacToe.Api.Models.Enums;

namespace TicTacToe.Api.Models
{
    public class GameResponse
    {
        public Guid Id { get; set; }
        public string?[][] Board { get; set; } = Array.Empty<string?[]>();
        public string CurrentPlayer { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string? Winner { get; set; }
        public List<MoveResponse> Moves { get; set; } = new();
        public List<int[]> WinningCells { get; set; } = new();
    }
}
