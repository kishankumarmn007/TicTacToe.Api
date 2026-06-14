using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Enums;
using TicTacToe.Api.Service;

namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        private readonly GameService service;

        public GamesController(GameService service)
        {
            this.service = service;
        }

        private GameResponse ToResponse(Game g)
        {
            var board = new string?[3][];
            for (var row = 0; row < 3; row++)
            {
                board[row] = new string?[3];
                for (var col = 0; col < 3; col++)
                {
                    board[row][col] = g.Board[row, col]?.ToString();
                }
            }

            return new GameResponse
            {
                Id = g.Id,
                Board = board,
                CurrentPlayer = g.CurrentPlayer.ToString(),
                Status = g.Status.ToString(),
                Mode = g.Mode.ToString(),
                Winner = g.Winner?.ToString(),
                Moves = g.Moves.Select(m => new MoveResponse
                {
                    MoveNumber = m.MoveNumber,
                    Player = m.Player.ToString(),
                    Row = m.Row,
                    Column = m.Column
                }).ToList(),
                WinningCells = g.WinningCells.Select(c => new[] { c.Item1, c.Item2 }).ToList()
            };
        }

        [HttpPost]
        public GameResponse Create([FromBody] GameCreateRequest request) => ToResponse(service.Create(request.Mode));

        [HttpGet("{id}")]
        public GameResponse Get(Guid id) => ToResponse(service.Get(id));

        [HttpPost("{id}/moves")]
        public GameResponse Move(Guid id, Move m) => ToResponse(service.Move(id, m.Row, m.Column));

        [HttpPost("{id}/undo")]
        public GameResponse Undo(Guid id) => ToResponse(service.Undo(id));

        [HttpPost("{id}/reset")]
        public GameResponse Reset(Guid id) => ToResponse(service.Reset(id));

        [HttpPost("{id}/skip")]
        public GameResponse Skip(Guid id) => ToResponse(service.Skip(id));
    }
}
