using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Enums;

namespace TicTacToe.Api.IntegrationTests
{
    public class GamesControllerIntegrationTests : IAsyncLifetime
    {
        private WebApplicationFactory<Program> factory = null!;
        private HttpClient client = null!;

        public async Task InitializeAsync()
        {
            factory = new WebApplicationFactory<Program>();
            client = factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            client?.Dispose();
            factory?.Dispose();
        }

        #region Create Tests

        [Fact]
        public async Task Create_WithTwoPlayerMode_ReturnsOkWithGameResponse()
        {
            // Arrange
            var request = new GameCreateRequest { Mode = GameMode.TwoPlayer };

            // Act
            var response = await client.PostAsJsonAsync("/api/games", request);
            var result = await response.Content.ReadAsAsync<GameResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("InProgress", result.Status);
            Assert.Equal("X", result.CurrentPlayer);
            Assert.NotNull(result.Board);
        }

        [Fact]
        public async Task Create_WithComputerMode_ReturnsOkWithGameResponse()
        {
            // Arrange
            var request = new GameCreateRequest { Mode = GameMode.Computer };

            // Act
            var response = await client.PostAsJsonAsync("/api/games", request);
            var result = await response.Content.ReadAsAsync<GameResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal("Computer", result.Mode);
        }

        #endregion

        #region Get Tests

        [Fact]
        public async Task Get_WithValidGameId_ReturnsOkWithGameResponse()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResponse = await client.PostAsJsonAsync("/api/games", createRequest);
            var game = await createResponse.Content.ReadAsAsync<GameResponse>();

            // Act
            var response = await client.GetAsync($"/api/games/{game.Id}");
            var result = await response.Content.ReadAsAsync<GameResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(game.Id, result.Id);
        }

        [Fact]
        public async Task Get_WithInvalidGameId_ReturnsInternalServerError()
        {
            // Act
            var response = await client.GetAsync($"/api/games/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        #endregion

        #region Move Tests

        [Fact]
        public async Task Move_WithValidMove_ReturnsOkWithUpdatedGame()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResponse = await client.PostAsJsonAsync("/api/games", createRequest);
            var game = await createResponse.Content.ReadAsAsync<GameResponse>();

            var moveRequest = new Move { Row = 0, Column = 0 };

            // Act
            var response = await client.PostAsJsonAsync($"/api/games/{game.Id}/moves", moveRequest);
            var result = await response.Content.ReadAsAsync<GameResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotNull(result.Board[0][0]);
            Assert.Equal("O", result.CurrentPlayer);
        }

        [Fact]
        public async Task Move_OnOccupiedCell_ReturnsInternalServerError()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResponse = await client.PostAsJsonAsync("/api/games", createRequest);
            var game = await createResponse.Content.ReadAsAsync<GameResponse>();

            var moveRequest = new Move { Row = 0, Column = 0 };

            // Make first move
            await client.PostAsJsonAsync($"/api/games/{game.Id}/moves", moveRequest);

            // Act - Try to move on same cell
            var response = await client.PostAsJsonAsync($"/api/games/{game.Id}/moves", moveRequest);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        #endregion

        #region Reset Tests

        [Fact]
        public async Task Reset_WithValidGameId_ReturnsOkWithResetGame()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResponse = await client.PostAsJsonAsync("/api/games", createRequest);
            var game = await createResponse.Content.ReadAsAsync<GameResponse>();

            // Make a move first
            var moveRequest = new Move { Row = 0, Column = 0 };
            await client.PostAsJsonAsync($"/api/games/{game.Id}/moves", moveRequest);

            // Act
            var response = await client.PostAsync($"/api/games/{game.Id}/reset", null);
            var result = await response.Content.ReadAsAsync<GameResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal("InProgress", result.Status);
            // Board should be empty after reset
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Assert.Null(result.Board[i][j]);
        }

        #endregion

        #region Skip Tests

        [Fact]
        public async Task Skip_WithValidGameId_ReturnsSwitchedPlayer()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResponse = await client.PostAsJsonAsync("/api/games", createRequest);
            var game = await createResponse.Content.ReadAsAsync<GameResponse>();
            var initialPlayer = game.CurrentPlayer;

            // Act
            var response = await client.PostAsync($"/api/games/{game.Id}/skip", null);
            var result = await response.Content.ReadAsAsync<GameResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotEqual(initialPlayer, result.CurrentPlayer);
        }

        #endregion

        #region Undo Tests

        [Fact]
        public async Task Undo_WithValidGameId_ReturnsGameWithUndoneMove()
        {
            // Arrange
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResponse = await client.PostAsJsonAsync("/api/games", createRequest);
            var game = await createResponse.Content.ReadAsAsync<GameResponse>();

            // Make a move
            var moveRequest = new Move { Row = 0, Column = 0 };
            var moveResponse = await client.PostAsJsonAsync($"/api/games/{game.Id}/moves", moveRequest);
            var movedGame = await moveResponse.Content.ReadAsAsync<GameResponse>();

            // Act
            var response = await client.PostAsync($"/api/games/{game.Id}/undo", null);
            var result = await response.Content.ReadAsAsync<GameResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Null(result.Board[0][0]);
            Assert.Equal("X", result.CurrentPlayer);
        }

        #endregion
    }
}
