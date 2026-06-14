using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models;
using TicTacToe.Api.Service;

namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("api/scoreboard")]
    public class ScoreboardController : ControllerBase
    {
        private readonly GameService service;

        public ScoreboardController(GameService service)
        {
            this.service = service;
        }

        [HttpGet]
        public Scoreboard Get() => service.GetScore();

        [HttpPost("reset")]
        public IActionResult Reset()
        {
            service.ResetScore();
            return NoContent();
        }
    }
}
