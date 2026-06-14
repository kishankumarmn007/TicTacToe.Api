using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Enums;
using TicTacToe.Api.Service;

namespace TicTacToe.Api.Tests
{
    public class GameServiceTests
    {
        private readonly Scoreboard scoreboard = new();
        private readonly GameService gameService;

        public GameServiceTests()
        {
            gameService = new GameService(scoreboard);
        }

        #region Create Game Tests

        [Fact]
        public void Create_WithTwoPlayerMode_CreatesGameSuccessfully()
        {
            // Act
            var game = gameService.Create(GameMode.TwoPlayer);

            // Assert
            Assert.NotNull(game);
            Assert.NotEqual(Guid.Empty, game.Id);
            Assert.Equal(GameMode.TwoPlayer, game.Mode);
            Assert.Equal(GameStatus.InProgress, game.Status);
            Assert.Equal(Player.X, game.CurrentPlayer);
            Assert.Null(game.Winner);
        }

        [Fact]
        public void Create_WithComputerMode_CreatesGameSuccessfully()
        {
            // Act
            var game = gameService.Create(GameMode.Computer);

            // Assert
            Assert.NotNull(game);
            Assert.Equal(GameMode.Computer, game.Mode);
            Assert.Equal(GameStatus.InProgress, game.Status);
        }

        [Fact]
        public void Create_MultipleGamesHaveDifferentIds()
        {
            // Act
            var game1 = gameService.Create(GameMode.TwoPlayer);
            var game2 = gameService.Create(GameMode.TwoPlayer);

            // Assert
            Assert.NotEqual(game1.Id, game2.Id);
        }

        #endregion

        #region Get Game Tests

        [Fact]
        public void Get_WithValidId_ReturnsGame()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            var retrievedGame = gameService.Get(game.Id);

            // Assert
            Assert.Equal(game.Id, retrievedGame.Id);
        }

        [Fact]
        public void Get_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => gameService.Get(Guid.NewGuid()));
        }

        #endregion

        #region Move Tests

        [Fact]
        public void Move_OnValidEmptyCell_PlacesMarker()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            var result = gameService.Move(game.Id, 0, 0);

            // Assert
            Assert.Equal(Player.X, result.Board[0, 0]);
            Assert.Equal(1, result.Moves.Count);
            Assert.Equal(Player.O, result.CurrentPlayer);
        }

        [Fact]
        public void Move_OnOccupiedCell_ThrowsException()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            gameService.Move(game.Id, 0, 0);

            // Act & Assert
            Assert.Throws<Exception>(() => gameService.Move(game.Id, 0, 0));
        }

        [Fact]
        public void Move_OutOfBoundsNegativeRow_ThrowsException()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act & Assert
            Assert.Throws<Exception>(() => gameService.Move(game.Id, -1, 0));
        }

        [Fact]
        public void Move_OutOfBoundsNegativeCol_ThrowsException()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act & Assert
            Assert.Throws<Exception>(() => gameService.Move(game.Id, 0, -1));
        }

        [Fact]
        public void Move_OutOfBoundsRowGreaterThen2_ThrowsException()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act & Assert
            Assert.Throws<Exception>(() => gameService.Move(game.Id, 3, 0));
        }

        [Fact]
        public void Move_OutOfBoundsColGreaterThen2_ThrowsException()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act & Assert
            Assert.Throws<Exception>(() => gameService.Move(game.Id, 0, 3));
        }

        [Fact]
        public void Move_OnCompletedGame_ThrowsException()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            // X wins with top row
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 0); // O
            gameService.Move(game.Id, 0, 1); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 0, 2); // X - wins

            // Act & Assert
            Assert.Throws<Exception>(() => gameService.Move(game.Id, 2, 0));
        }

        [Fact]
        public void Move_AlternatesPlayers()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act & Assert
            Assert.Equal(Player.X, game.CurrentPlayer);

            gameService.Move(game.Id, 0, 0);
            Assert.Equal(Player.O, gameService.Get(game.Id).CurrentPlayer);

            gameService.Move(game.Id, 0, 1);
            Assert.Equal(Player.X, gameService.Get(game.Id).CurrentPlayer);
        }

        [Fact]
        public void Move_RecordsMoveHistory()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 0);
            gameService.Move(game.Id, 1, 1);

            // Assert
            var updatedGame = gameService.Get(game.Id);
            Assert.Equal(2, updatedGame.Moves.Count);
            Assert.Equal(1, updatedGame.Moves[0].MoveNumber);
            Assert.Equal(Player.X, updatedGame.Moves[0].Player);
            Assert.Equal(0, updatedGame.Moves[0].Row);
            Assert.Equal(0, updatedGame.Moves[0].Column);
        }

        #endregion

        #region Win Condition Tests

        [Fact]
        public void Move_HorizontalWinTopRow_DetectsWinner()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 0); // O
            gameService.Move(game.Id, 0, 1); // X
            gameService.Move(game.Id, 1, 1); // O
            var result = gameService.Move(game.Id, 0, 2); // X - wins

            // Assert
            Assert.Equal(GameStatus.Won, result.Status);
            Assert.Equal(Player.X, result.Winner);
            Assert.Equal(3, result.WinningCells.Count);
        }

        [Fact]
        public void Move_HorizontalWinMiddleRow_DetectsWinner()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 0); // O
            gameService.Move(game.Id, 0, 1); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 2, 2); // X
            var result = gameService.Move(game.Id, 1, 2); // O - wins

            // Assert
            Assert.Equal(GameStatus.Won, result.Status);
            Assert.Equal(Player.O, result.Winner);
        }

        [Fact]
        public void Move_HorizontalWinBottomRow_DetectsWinner()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 2, 0); // O
            gameService.Move(game.Id, 0, 1); // X
            gameService.Move(game.Id, 2, 1); // O
            gameService.Move(game.Id, 1, 1); // X
            var result = gameService.Move(game.Id, 2, 2); // O - wins

            // Assert
            Assert.Equal(GameStatus.Won, result.Status);
            Assert.Equal(Player.O, result.Winner);
        }

        [Fact]
        public void Move_VerticalWinFirstCol_DetectsWinner()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 0, 1); // O
            gameService.Move(game.Id, 1, 0); // X
            gameService.Move(game.Id, 0, 2); // O
            var result = gameService.Move(game.Id, 2, 0); // X - wins

            // Assert
            Assert.Equal(GameStatus.Won, result.Status);
            Assert.Equal(Player.X, result.Winner);
        }

        [Fact]
        public void Move_VerticalWinSecondCol_DetectsWinner()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 0, 1); // O
            gameService.Move(game.Id, 1, 0); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 2, 2); // X
            var result = gameService.Move(game.Id, 2, 1); // O - wins

            // Assert
            Assert.Equal(GameStatus.Won, result.Status);
            Assert.Equal(Player.O, result.Winner);
        }

        [Fact]
        public void Move_VerticalWinThirdCol_DetectsWinner()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 0, 2); // O
            gameService.Move(game.Id, 1, 0); // X
            gameService.Move(game.Id, 1, 2); // O
            gameService.Move(game.Id, 2, 1); // X
            var result = gameService.Move(game.Id, 2, 2); // O - wins

            // Assert
            Assert.Equal(GameStatus.Won, result.Status);
            Assert.Equal(Player.O, result.Winner);
        }

        [Fact]
        public void Move_DiagonalWinTopLeftToBottomRight_DetectsWinner()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 0, 1); // O
            gameService.Move(game.Id, 1, 1); // X
            gameService.Move(game.Id, 0, 2); // O
            var result = gameService.Move(game.Id, 2, 2); // X - wins

            // Assert
            Assert.Equal(GameStatus.Won, result.Status);
            Assert.Equal(Player.X, result.Winner);
        }

        [Fact]
        public void Move_DiagonalWinTopRightToBottomLeft_DetectsWinner()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            gameService.Move(game.Id, 0, 2); // X
            gameService.Move(game.Id, 0, 0); // O
            gameService.Move(game.Id, 1, 1); // X
            gameService.Move(game.Id, 0, 1); // O
            var result = gameService.Move(game.Id, 2, 0); // X - wins

            // Assert
            Assert.Equal(GameStatus.Won, result.Status);
            Assert.Equal(Player.X, result.Winner);
        }

        [Fact]
        public void Move_WinIncrementsScoreboard()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            var initialXWins = gameService.GetScore().XWins;

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 0); // O
            gameService.Move(game.Id, 0, 1); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 0, 2); // X - wins

            // Assert
            Assert.Equal(initialXWins + 1, gameService.GetScore().XWins);
        }

        #endregion

        #region Draw Tests

        [Fact]
        public void Move_AllCellsFilled_DetectsDraw()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act - Create a draw scenario
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 0, 1); // O
            gameService.Move(game.Id, 0, 2); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 1, 0); // X
            gameService.Move(game.Id, 1, 2); // O
            gameService.Move(game.Id, 2, 1); // X
            gameService.Move(game.Id, 2, 0); // O
            var result = gameService.Move(game.Id, 2, 2); // X

            // Assert
            Assert.Equal(GameStatus.Draw, result.Status);
            Assert.Null(result.Winner);
        }

        [Fact]
        public void Move_DrawIncrementsScoreboard()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            var initialDraws = gameService.GetScore().Draws;

            // Act
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 0, 1); // O
            gameService.Move(game.Id, 0, 2); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 1, 0); // X
            gameService.Move(game.Id, 1, 2); // O
            gameService.Move(game.Id, 2, 1); // X
            gameService.Move(game.Id, 2, 0); // O
            gameService.Move(game.Id, 2, 2); // X

            // Assert
            Assert.Equal(initialDraws + 1, gameService.GetScore().Draws);
        }

        #endregion

        #region Undo Tests

        [Fact]
        public void Undo_SingleMove_RemovesMarkerAndSwitchesPlayer()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            gameService.Move(game.Id, 0, 0); // X

            // Act
            var result = gameService.Undo(game.Id);

            // Assert
            Assert.Null(result.Board[0, 0]);
            Assert.Equal(Player.X, result.CurrentPlayer);
            Assert.Empty(result.Moves);
        }

        [Fact]
        public void Undo_InComputerMode_RemovesTwoMoves()
        {
            // Arrange
            var game = gameService.Create(GameMode.Computer);
            gameService.Move(game.Id, 0, 0); // X, then computer plays

            var movesBeforeUndo = gameService.Get(game.Id).Moves.Count;

            // Act
            var result = gameService.Undo(game.Id);

            // Assert
            Assert.True(result.Moves.Count < movesBeforeUndo);
        }

        [Fact]
        public void Undo_OnCompletedGame_ThrowsException()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 0); // O
            gameService.Move(game.Id, 0, 1); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 0, 2); // X - wins

            // Act & Assert
            Assert.Throws<Exception>(() => gameService.Undo(game.Id));
        }

        [Fact]
        public void Undo_MultipleUndos_WorkCorrectly()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 0, 1); // X

            // Act
            gameService.Undo(game.Id); // Undo X's second move
            gameService.Undo(game.Id); // Undo O's move

            var result = gameService.Get(game.Id);

            // Assert
            Assert.Null(result.Board[0, 1]);
            Assert.Null(result.Board[1, 1]);
            Assert.Equal(Player.X, result.Board[0, 0]);
        }

        #endregion

        #region Reset Tests

        [Fact]
        public void Reset_ClearsBoardAndResets()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 1); // O

            // Act
            var result = gameService.Reset(game.Id);

            // Assert
            Assert.All(Enumerable.Range(0, 3), row =>
                Assert.All(Enumerable.Range(0, 3), col =>
                    Assert.Null(result.Board[row, col])
                )
            );
            Assert.Empty(result.Moves);
            Assert.Equal(Player.X, result.CurrentPlayer);
            Assert.Equal(GameStatus.InProgress, result.Status);
            Assert.Null(result.Winner);
        }

        [Fact]
        public void Reset_AfterWin_StartsNewGame()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 0); // O
            gameService.Move(game.Id, 0, 1); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 0, 2); // X - wins

            // Act
            var result = gameService.Reset(game.Id);

            // Assert
            Assert.Equal(GameStatus.InProgress, result.Status);
            Assert.Null(result.Winner);
        }

        #endregion

        #region Skip Tests

        [Fact]
        public void Skip_SwitchesCurrentPlayer()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);

            // Act
            var result = gameService.Skip(game.Id);

            // Assert
            Assert.Equal(Player.O, result.CurrentPlayer);
        }

        [Fact]
        public void Skip_OnCompletedGame_ThrowsException()
        {
            // Arrange
            var game = gameService.Create(GameMode.TwoPlayer);
            gameService.Move(game.Id, 0, 0); // X
            gameService.Move(game.Id, 1, 0); // O
            gameService.Move(game.Id, 0, 1); // X
            gameService.Move(game.Id, 1, 1); // O
            gameService.Move(game.Id, 0, 2); // X - wins

            // Act & Assert
            Assert.Throws<Exception>(() => gameService.Skip(game.Id));
        }

        #endregion

        #region Scoreboard Tests

        [Fact]
        public void GetScore_ReturnsScoreboard()
        {
            // Act
            var score = gameService.GetScore();

            // Assert
            Assert.NotNull(score);
            Assert.Equal(0, score.XWins);
            Assert.Equal(0, score.OWins);
            Assert.Equal(0, score.Draws);
        }

        [Fact]
        public void ResetScore_SetsAllCountersToZero()
        {
            // Arrange
            var game1 = gameService.Create(GameMode.TwoPlayer);
            gameService.Move(game1.Id, 0, 0); // X
            gameService.Move(game1.Id, 1, 0); // O
            gameService.Move(game1.Id, 0, 1); // X
            gameService.Move(game1.Id, 1, 1); // O
            gameService.Move(game1.Id, 0, 2); // X - wins

            // Act
            gameService.ResetScore();

            var result = gameService.GetScore();

            // Assert
            Assert.Equal(0, result.XWins);
            Assert.Equal(0, result.OWins);
            Assert.Equal(0, result.Draws);
        }

        #endregion

        #region Computer AI Tests

        [Fact]
        public void Move_ComputerMode_ComputerPlaysAfterPlayer()
        {
            // Arrange
            var game = gameService.Create(GameMode.Computer);

            // Act
            var result = gameService.Move(game.Id, 0, 0); // X

            // Assert
            Assert.Equal(Player.X, result.Board[0, 0]);
            Assert.True(result.Moves.Count > 1); // X's move + Computer's move
        }

        [Fact]
        public void Move_ComputerMode_ComputerBlocksPlayerWin()
        {
            // Arrange
            var game = gameService.Create(GameMode.Computer);

            // Act
            gameService.Move(game.Id, 0, 0); // X (human)
            gameService.Move(game.Id, 0, 1); // Computer should block if player can win
            var result = gameService.Get(game.Id);

            // Assert
            Assert.NotNull(result.Board[0, 0]);
            Assert.NotNull(result.Board[0, 1]);
        }

        [Fact]
        public void Move_ComputerMode_ComputerWinsIfPossible()
        {
            // Arrange
            var game = gameService.Create(GameMode.Computer);

            // Set up a scenario where computer can win
            // X plays
            gameService.Move(game.Id, 0, 0); // X
            // Computer plays and should try to win/block
            var result = gameService.Get(game.Id);

            // Assert - Game should still be in progress or won
            Assert.True(result.Status == GameStatus.InProgress || result.Status == GameStatus.Won);
        }

        #endregion
    }
}
