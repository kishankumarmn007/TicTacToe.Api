using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Enums;

namespace TicTacToe.Api.Tests
{
    public class GameModelTests
    {
        #region Game Initialization Tests

        [Fact]
        public void Game_InitializesWithValidDefaults()
        {
            // Act
            var game = new Game();

            // Assert
            Assert.NotEqual(Guid.Empty, game.Id);
            Assert.NotNull(game.Board);
            Assert.Equal(3, game.Board.GetLength(0));
            Assert.Equal(3, game.Board.GetLength(1));
            Assert.Equal(Player.X, game.CurrentPlayer);
            Assert.Equal(GameStatus.InProgress, game.Status);
            Assert.Null(game.Winner);
            Assert.NotNull(game.Moves);
            Assert.Empty(game.Moves);
            Assert.NotNull(game.WinningCells);
            Assert.Empty(game.WinningCells);
        }

        [Fact]
        public void Game_BoardIsEmptyOnCreation()
        {
            // Act
            var game = new Game();

            // Assert
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Assert.Null(game.Board[i, j]);
                }
            }
        }

        [Fact]
        public void Game_EachGameHasUniqueId()
        {
            // Act
            var game1 = new Game();
            var game2 = new Game();

            // Assert
            Assert.NotEqual(game1.Id, game2.Id);
        }

        #endregion

        #region Board Manipulation Tests

        [Fact]
        public void Game_BoardCanStorePlayerMarkers()
        {
            // Arrange
            var game = new Game();

            // Act
            game.Board[0, 0] = Player.X;
            game.Board[1, 1] = Player.O;

            // Assert
            Assert.Equal(Player.X, game.Board[0, 0]);
            Assert.Equal(Player.O, game.Board[1, 1]);
        }

        [Fact]
        public void Game_AllBoardPositionsCanBeAccessed()
        {
            // Arrange
            var game = new Game();

            // Act & Assert
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    game.Board[i, j] = Player.X;
                    Assert.Equal(Player.X, game.Board[i, j]);
                    game.Board[i, j] = Player.O;
                    Assert.Equal(Player.O, game.Board[i, j]);
                    game.Board[i, j] = null;
                }
            }
        }

        #endregion

        #region Move History Tests

        [Fact]
        public void Game_MovesListStartsEmpty()
        {
            // Act
            var game = new Game();

            // Assert
            Assert.NotNull(game.Moves);
            Assert.Empty(game.Moves);
        }

        [Fact]
        public void Game_CanAddMovesToHistory()
        {
            // Arrange
            var game = new Game();
            var move = new Move { MoveNumber = 1, Player = Player.X, Row = 0, Column = 0 };

            // Act
            game.Moves.Add(move);

            // Assert
            Assert.Single(game.Moves);
            Assert.Equal(1, game.Moves[0].MoveNumber);
            Assert.Equal(Player.X, game.Moves[0].Player);
        }

        [Fact]
        public void Game_CanClearMoveHistory()
        {
            // Arrange
            var game = new Game();
            game.Moves.Add(new Move { MoveNumber = 1, Player = Player.X, Row = 0, Column = 0 });
            game.Moves.Add(new Move { MoveNumber = 2, Player = Player.O, Row = 1, Column = 1 });

            // Act
            game.Moves.Clear();

            // Assert
            Assert.Empty(game.Moves);
        }

        #endregion

        #region Status and Winner Tests

        [Fact]
        public void Game_StatusCanBeChanged()
        {
            // Arrange
            var game = new Game();
            Assert.Equal(GameStatus.InProgress, game.Status);

            // Act
            game.Status = GameStatus.Won;

            // Assert
            Assert.Equal(GameStatus.Won, game.Status);
        }

        [Fact]
        public void Game_CanSetWinner()
        {
            // Arrange
            var game = new Game();
            Assert.Null(game.Winner);

            // Act
            game.Winner = Player.X;

            // Assert
            Assert.Equal(Player.X, game.Winner);
        }

        [Fact]
        public void Game_SetWinnerToNull()
        {
            // Arrange
            var game = new Game { Winner = Player.X };

            // Act
            game.Winner = null;

            // Assert
            Assert.Null(game.Winner);
        }

        #endregion

        #region Winning Cells Tests

        [Fact]
        public void Game_WinningCellsStartEmpty()
        {
            // Act
            var game = new Game();

            // Assert
            Assert.NotNull(game.WinningCells);
            Assert.Empty(game.WinningCells);
        }

        [Fact]
        public void Game_CanAddWinningCells()
        {
            // Arrange
            var game = new Game();

            // Act
            game.WinningCells.Add((0, 0));
            game.WinningCells.Add((0, 1));
            game.WinningCells.Add((0, 2));

            // Assert
            Assert.Equal(3, game.WinningCells.Count);
            Assert.Contains((0, 0), game.WinningCells);
            Assert.Contains((0, 1), game.WinningCells);
            Assert.Contains((0, 2), game.WinningCells);
        }

        [Fact]
        public void Game_CanClearWinningCells()
        {
            // Arrange
            var game = new Game();
            game.WinningCells.Add((0, 0));
            game.WinningCells.Add((0, 1));

            // Act
            game.WinningCells.Clear();

            // Assert
            Assert.Empty(game.WinningCells);
        }

        #endregion

        #region Mode Tests

        [Fact]
        public void Game_CanSetGameMode()
        {
            // Arrange
            var game = new Game();

            // Act
            game.Mode = GameMode.Computer;

            // Assert
            Assert.Equal(GameMode.Computer, game.Mode);
        }

        [Fact]
        public void Game_CanHaveTwoPlayerMode()
        {
            // Arrange
            var game = new Game();

            // Act
            game.Mode = GameMode.TwoPlayer;

            // Assert
            Assert.Equal(GameMode.TwoPlayer, game.Mode);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Game_CurrentPlayerCanBeChanged()
        {
            // Arrange
            var game = new Game();
            Assert.Equal(Player.X, game.CurrentPlayer);

            // Act
            game.CurrentPlayer = Player.O;

            // Assert
            Assert.Equal(Player.O, game.CurrentPlayer);
        }

        [Fact]
        public void Game_AllPropertiesAreReadWritable()
        {
            // Arrange
            var game = new Game();
            var newId = Guid.NewGuid();
            var newBoard = new Player?[3, 3];

            // Act
            game.Id = newId;
            game.Board = newBoard;
            game.CurrentPlayer = Player.O;
            game.Status = GameStatus.Draw;
            game.Winner = Player.O;
            game.Mode = GameMode.Computer;

            // Assert
            Assert.Equal(newId, game.Id);
            Assert.Equal(newBoard, game.Board);
            Assert.Equal(Player.O, game.CurrentPlayer);
            Assert.Equal(GameStatus.Draw, game.Status);
            Assert.Equal(Player.O, game.Winner);
            Assert.Equal(GameMode.Computer, game.Mode);
        }

        #endregion
    }

    public class MoveModelTests
    {
        [Fact]
        public void Move_CanBeCreated()
        {
            // Act
            var move = new Move { MoveNumber = 1, Player = Player.X, Row = 0, Column = 0 };

            // Assert
            Assert.Equal(1, move.MoveNumber);
            Assert.Equal(Player.X, move.Player);
            Assert.Equal(0, move.Row);
            Assert.Equal(0, move.Column);
        }

        [Fact]
        public void Move_AllPropertiesAreReadWritable()
        {
            // Arrange
            var move = new Move();

            // Act
            move.MoveNumber = 5;
            move.Player = Player.O;
            move.Row = 2;
            move.Column = 2;

            // Assert
            Assert.Equal(5, move.MoveNumber);
            Assert.Equal(Player.O, move.Player);
            Assert.Equal(2, move.Row);
            Assert.Equal(2, move.Column);
        }
    }

    public class ScoreboardModelTests
    {
        [Fact]
        public void Scoreboard_InitializesWithZeros()
        {
            // Act
            var scoreboard = new Scoreboard();

            // Assert
            Assert.Equal(0, scoreboard.XWins);
            Assert.Equal(0, scoreboard.OWins);
            Assert.Equal(0, scoreboard.Draws);
        }

        [Fact]
        public void Scoreboard_CanIncrementXWins()
        {
            // Arrange
            var scoreboard = new Scoreboard();

            // Act
            scoreboard.XWins++;

            // Assert
            Assert.Equal(1, scoreboard.XWins);
        }

        [Fact]
        public void Scoreboard_CanIncrementOWins()
        {
            // Arrange
            var scoreboard = new Scoreboard();

            // Act
            scoreboard.OWins++;

            // Assert
            Assert.Equal(1, scoreboard.OWins);
        }

        [Fact]
        public void Scoreboard_CanIncrementDraws()
        {
            // Arrange
            var scoreboard = new Scoreboard();

            // Act
            scoreboard.Draws++;

            // Assert
            Assert.Equal(1, scoreboard.Draws);
        }

        [Fact]
        public void Scoreboard_CanSetAllValues()
        {
            // Act
            var scoreboard = new Scoreboard { XWins = 5, OWins = 3, Draws = 2 };

            // Assert
            Assert.Equal(5, scoreboard.XWins);
            Assert.Equal(3, scoreboard.OWins);
            Assert.Equal(2, scoreboard.Draws);
        }

        [Fact]
        public void Scoreboard_CanResetToZero()
        {
            // Arrange
            var scoreboard = new Scoreboard { XWins = 10, OWins = 8, Draws = 5 };

            // Act
            scoreboard.XWins = 0;
            scoreboard.OWins = 0;
            scoreboard.Draws = 0;

            // Assert
            Assert.Equal(0, scoreboard.XWins);
            Assert.Equal(0, scoreboard.OWins);
            Assert.Equal(0, scoreboard.Draws);
        }
    }

    public class EnumTests
    {
        [Fact]
        public void GameMode_HasTwoPlayerValue()
        {
            // Assert
            Assert.Equal(GameMode.TwoPlayer, GameMode.TwoPlayer);
        }

        [Fact]
        public void GameMode_HasComputerValue()
        {
            // Assert
            Assert.Equal(GameMode.Computer, GameMode.Computer);
        }

        [Fact]
        public void GameStatus_HasInProgressValue()
        {
            // Assert
            Assert.Equal(GameStatus.InProgress, GameStatus.InProgress);
        }

        [Fact]
        public void GameStatus_HasWonValue()
        {
            // Assert
            Assert.Equal(GameStatus.Won, GameStatus.Won);
        }

        [Fact]
        public void GameStatus_HasDrawValue()
        {
            // Assert
            Assert.Equal(GameStatus.Draw, GameStatus.Draw);
        }

        [Fact]
        public void Player_HasXValue()
        {
            // Assert
            Assert.Equal(Player.X, Player.X);
        }

        [Fact]
        public void Player_HasOValue()
        {
            // Assert
            Assert.Equal(Player.O, Player.O);
        }
    }
}
