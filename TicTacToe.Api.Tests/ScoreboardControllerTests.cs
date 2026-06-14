using TicTacToe.Api.Controllers;
using TicTacToe.Api.Models;
using TicTacToe.Api.Service;

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

        #region Get Tests

        [Fact]
        public void Get_ReturnsScoreboard()
        {
            // Arrange
            var scoreboard = new Scoreboard { XWins = 5, OWins = 3, Draws = 2 };
            mockGameService.Setup(s => s.GetScore()).Returns(scoreboard);

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.XWins);
            Assert.Equal(3, result.OWins);
            Assert.Equal(2, result.Draws);
            mockGameService.Verify(s => s.GetScore(), Times.Once);
        }

        [Fact]
        public void Get_ReturnsZeroScoresForNewGame()
        {
            // Arrange
            var scoreboard = new Scoreboard { XWins = 0, OWins = 0, Draws = 0 };
            mockGameService.Setup(s => s.GetScore()).Returns(scoreboard);

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
            var scoreboard = new Scoreboard { XWins = 10, OWins = 8, Draws = 5 };
            mockGameService.Setup(s => s.GetScore()).Returns(scoreboard);

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
        public void Reset_CallsResetScoreOnService()
        {
            // Act
            var result = controller.Reset();

            // Assert
            mockGameService.Verify(s => s.ResetScore(), Times.Once);
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

        [Fact]
        public void Reset_ResetsAllScores()
        {
            // Arrange
            var scoreboard = new Scoreboard { XWins = 0, OWins = 0, Draws = 0 };
            mockGameService.Setup(s => s.GetScore()).Returns(scoreboard);

            // Act
            controller.Reset();
            var result = controller.Get();

            // Assert
            Assert.Equal(0, result.XWins);
            Assert.Equal(0, result.OWins);
            Assert.Equal(0, result.Draws);
            mockGameService.Verify(s => s.ResetScore(), Times.Once);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Get_AfterReset_ReturnsZeroScores()
        {
            // Arrange
            var scoreboard = new Scoreboard { XWins = 5, OWins = 3, Draws = 2 };
            mockGameService.Setup(s => s.GetScore()).Returns(scoreboard);

            // First get should show the scores
            var initialResult = controller.Get();
            Assert.Equal(5, initialResult.XWins);

            // Now reset
            scoreboard = new Scoreboard { XWins = 0, OWins = 0, Draws = 0 };
            mockGameService.Setup(s => s.GetScore()).Returns(scoreboard);
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
