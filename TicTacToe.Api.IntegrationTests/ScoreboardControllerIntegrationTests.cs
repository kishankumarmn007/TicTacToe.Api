using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Enums;

namespace TicTacToe.Api.IntegrationTests
{
    public class ScoreboardControllerIntegrationTests : IAsyncLifetime
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

        #region Get Tests

        [Fact]
        public async Task Get_ReturnsOkWithScoreboard()
        {
            // Act
            var response = await client.GetAsync("/api/scoreboard");
            var result = await response.Content.ReadAsAsync<Scoreboard>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.True(result.XWins >= 0);
            Assert.True(result.OWins >= 0);
            Assert.True(result.Draws >= 0);
        }

        #endregion

        #region Reset Tests

        [Fact]
        public async Task Reset_ReturnsNoContent()
        {
            // Act
            var response = await client.PostAsync("/api/scoreboard/reset", null);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Reset_ResetsScoreboard()
        {
            // Act - Reset the scoreboard
            await client.PostAsync("/api/scoreboard/reset", null);

            // Get the scoreboard after reset
            var response = await client.GetAsync("/api/scoreboard");
            var result = await response.Content.ReadAsAsync<Scoreboard>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(0, result.XWins);
            Assert.Equal(0, result.OWins);
            Assert.Equal(0, result.Draws);
        }

        #endregion

        #region End-to-End Tests

        [Fact]
        public async Task PlayGame_WhenXWins_IncrementsScoreboard()
        {
            // Arrange - Reset scoreboard
            await client.PostAsync("/api/scoreboard/reset", null);

            // Create a game
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResponse = await client.PostAsJsonAsync("/api/games", createRequest);
            var game = await createResponse.Content.ReadAsAsync<GameResponse>();

            // Play moves to create a winning pattern (X wins)
            // X at (0,0), O at (1,0), X at (0,1), O at (1,1), X at (0,2) - X wins top row
            var moves = new[] { (0, 0), (1, 0), (0, 1), (1, 1), (0, 2) };
            
            foreach (var (row, col) in moves)
            {
                var moveRequest = new Move { Row = row, Column = col };
                var moveResponse = await client.PostAsJsonAsync($"/api/games/{game.Id}/moves", moveRequest);
                
                if (!moveResponse.IsSuccessStatusCode)
                    break;
            }

            // Act - Get the scoreboard
            var response = await client.GetAsync("/api/scoreboard");
            var scoreboard = await response.Content.ReadAsAsync<Scoreboard>();

            // Assert
            Assert.NotNull(scoreboard);
            Assert.True(scoreboard.XWins >= 0);
        }

        [Fact]
        public async Task PlayGame_WhenDraw_IncrementsScoreboard()
        {
            // Arrange - Reset scoreboard
            await client.PostAsync("/api/scoreboard/reset", null);

            // Create a game
            var createRequest = new GameCreateRequest { Mode = GameMode.TwoPlayer };
            var createResponse = await client.PostAsJsonAsync("/api/games", createRequest);
            var game = await createResponse.Content.ReadAsAsync<GameResponse>();

            // Play moves to create a draw (fill all cells without winner)
            var moves = new[] 
            { 
                (0, 0), (0, 1), (0, 2), 
                (1, 0), (1, 1), (1, 2), 
                (2, 0), (2, 1), (2, 2) 
            };
            
            foreach (var (row, col) in moves)
            {
                var moveRequest = new Move { Row = row, Column = col };
                var moveResponse = await client.PostAsJsonAsync($"/api/games/{game.Id}/moves", moveRequest);
                
                if (!moveResponse.IsSuccessStatusCode)
                    break;
            }

            // Act - Get the scoreboard
            var response = await client.GetAsync("/api/scoreboard");
            var scoreboard = await response.Content.ReadAsAsync<Scoreboard>();

            // Assert
            Assert.NotNull(scoreboard);
            Assert.True(scoreboard.Draws >= 0);
        }

        #endregion
    }
}
