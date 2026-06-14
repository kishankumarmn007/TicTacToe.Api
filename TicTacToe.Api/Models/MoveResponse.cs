namespace TicTacToe.Api.Models
{
    public class MoveResponse
    {
        public int MoveNumber { get; set; }
        public string Player { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
