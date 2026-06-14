using TicTacToe.Api.Controllers;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Enums;
using TicTacToe.Api.Service;

namespace TicTacToe.Api.Tests
{
    public class GamesControllerTests
    {
        private readonly Scoreboard scoreboard = new();
        private readonly GameService gameService;
        private readonly GamesController controller;

        public GamesControllerTests()
        {
            gameService = new GameService(scoreboard);
            controller = new GamesController(gameService);
        }

        #region Create Tests

        [Fact]
        public void Create_WithValidRequest_ReturnsGameResponse()
        {
            // Arrange
            var request = new GameCreateRequest { Mode = GameMode.TwoPlayer };

            // Act
            var result = controller.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(GameMode.TwoPlayer.ToString(), result.Mode);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public void Create_WithComputerMode_ReturnsGameResponse()
        {
            // Arrange
            var request = new GameCreateRequest { Mode = GameMode.Computer };

            // Act
            var result = controller.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(GameMode.Computer.ToString(), result.Mode);
        }

        #endregion

        #region Get Tests

        [Fact]
        public void Get_WithValidId_ReturnsGameResponse()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;

            // Act
            var result = controller.Get(gameId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.Id);
        }

        [Fact]
        public void Get_ReturnsCorrectBoardFormat()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;

            // Act
            var result = controller.Get(gameId);

            // Assert
            Assert.NotNull(result.Board);
            Assert.Equal(3, result.Board.Length);
        }

        #endregion

        #region Move Tests

        [Fact]
        public void Move_WithValidMove_ReturnUpdatedGame()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;
            var move = new Move { Row = 0, Column = 0 };

            // Act
            var result = controller.Move(gameId, move);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("X", result.Board[0][0]);
        }

        [Fact]
        public void Move_ReturnsGameWithUpdatedStatus()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;
            var move = new Move { Row = 0, Column = 0 };

            // Act
            var result = controller.Move(gameId, move);

            // Assert
            Assert.Equal(GameStatus.InProgress.ToString(), result.Status);
        }

        #endregion

        #region Undo Tests

        [Fact]
        public void Undo_WithValidGameId_ReturnsUpdatedGame()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;
            controller.Move(gameId, new Move { Row = 0, Column = 0 });

            // Act
            var result = controller.Undo(gameId);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Board[0][0]);
        }

        #endregion

        #region Reset Tests

        [Fact]
        public void Reset_WithValidGameId_ReturnsResetGame()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;
            controller.Move(gameId, new Move { Row = 0, Column = 0 });

            // Act
            var result = controller.Reset(gameId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.Id);
        }

        [Fact]
        public void Reset_ReturnsGameWithEmptyBoard()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;
            controller.Move(gameId, new Move { Row = 0, Column = 0 });

            // Act
            var result = controller.Reset(gameId);

            // Assert
            Assert.NotNull(result.Board);
            Assert.All(Enumerable.Range(0, 3), row =>
                Assert.All(Enumerable.Range(0, 3), col =>
                    Assert.Null(result.Board[row][col])
                )
            );
        }

        [Fact]
        public void Reset_ReturnsGameInProgress()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;

            // Act
            var result = controller.Reset(gameId);

            // Assert
            Assert.Equal(GameStatus.InProgress.ToString(), result.Status);
        }

        #endregion

        #region Skip Tests

        [Fact]
        public void Skip_WithValidGameId_ReturnUpdatedGame()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;

            // Act
            var result = controller.Skip(gameId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.Id);
        }

        [Fact]
        public void Skip_ReturnGameWithSwitchedPlayer()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;

            // Act
            var result = controller.Skip(gameId);

            // Assert
            Assert.Equal("O", result.CurrentPlayer);
        }

        #endregion

        #region GameResponse Tests

        [Fact]
        public void Create_ReturnedResponseHasAllRequiredFields()
        {
            // Arrange
            var request = new GameCreateRequest { Mode = GameMode.TwoPlayer };

            // Act
            var result = controller.Create(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.NotNull(result.Board);
            Assert.NotNull(result.CurrentPlayer);
            Assert.NotNull(result.Status);
            Assert.NotNull(result.Mode);
            Assert.NotNull(result.Moves);
            Assert.NotNull(result.WinningCells);
        }

        [Fact]
        public void GameResponse_IncludesMoveHistory()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;
            controller.Move(gameId, new Move { Row = 0, Column = 0 });
            controller.Move(gameId, new Move { Row = 1, Column = 1 });

            // Act
            var result = controller.Get(gameId);

            // Assert
            Assert.Equal(2, result.Moves.Count);
            Assert.Equal("X", result.Moves[0].Player);
            Assert.Equal(1, result.Moves[0].MoveNumber);
        }

        [Fact]
        public void GameResponse_IncludesWinningCells()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResult = controller.Create(createRequest);
            var gameId = createResult.Id;
            // Create a winning position
            controller.Move(gameId, new Move { Row = 0, Column = 0 }); // X
            controller.Move(gameId, new Move { Row = 1, Column = 0 }); // O
            controller.Move(gameId, new Move { Row = 0, Column = 1 }); // X
            controller.Move(gameId, new Move { Row = 1, Column = 1 }); // O
            controller.Move(gameId, new Move { Row = 0, Column = 2 }); // X wins

            // Act
            var result = controller.Get(gameId);

            // Assert
            Assert.True(result.WinningCells.Count > 0);
            Assert.NotNull(result.WinningCells);
        }

        #endregion
    }
}
