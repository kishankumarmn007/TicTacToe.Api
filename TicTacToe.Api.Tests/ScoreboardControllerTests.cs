using TicTacToe.Api.Controllers;
using TicTacToe.Api.Models;
using TicTacToe.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.Api.Tests
{
    public class ScoreboardControllerTests
    {
        private readonly Scoreboard scoreboard = new();
        private readonly GameService gameService;
        private readonly ScoreboardController controller;

        public ScoreboardControllerTests()
        {
            gameService = new GameService(scoreboard);
            controller = new ScoreboardController(gameService);
        }

        #region Get Tests

        [Fact]
        public void Get_ReturnsScoreboard()
        {
            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.XWins);
            Assert.Equal(0, result.OWins);
            Assert.Equal(0, result.Draws);
        }

        [Fact]
        public void Get_ReturnsZeroScoresForNewGame()
        {
            // Act
            var result = controller.Get();

            // Assert
            Assert.Equal(0, result.XWins);
            Assert.Equal(0, result.OWins);
            Assert.Equal(0, result.Draws);
        }

        [Fact]
        public void Get_ReturnsCorrectScores()
        {
            // Arrange
            scoreboard.XWins = 10;
            scoreboard.OWins = 8;
            scoreboard.Draws = 5;

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.XWins);
            Assert.Equal(8, result.OWins);
            Assert.Equal(5, result.Draws);
        }

        #endregion

        #region Reset Tests

        [Fact]
        public void Reset_ResetsAllScores()
        {
            // Arrange
            scoreboard.XWins = 5;
            scoreboard.OWins = 3;
            scoreboard.Draws = 2;

            // Act
            var resetResult = controller.Reset();
            var scoreResult = controller.Get();

            // Assert
            Assert.Equal(0, scoreResult.XWins);
            Assert.Equal(0, scoreResult.OWins);
            Assert.Equal(0, scoreResult.Draws);
        }

        [Fact]
        public void Reset_ReturnsNoContent()
        {
            // Act
            var result = controller.Reset();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Reset_StatusCodeIsNoContent()
        {
            // Act
            var result = controller.Reset() as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Get_AfterReset_ReturnsZeroScores()
        {
            // Arrange
            scoreboard.XWins = 5;
            scoreboard.OWins = 3;
            scoreboard.Draws = 2;

            // First get should show the scores
            var initialResult = controller.Get();
            Assert.Equal(5, initialResult.XWins);

            // Now reset
            controller.Reset();

            // Act
            var finalResult = controller.Get();

            // Assert
            Assert.Equal(0, finalResult.XWins);
            Assert.Equal(0, finalResult.OWins);
            Assert.Equal(0, finalResult.Draws);
        }

        #endregion
    }
}
