# TicTacToe.Api

A comprehensive Tic Tac Toe API built with ASP.NET Core (.NET 10) that provides game management, scoreboard tracking, and both single-player (vs Computer AI) and multiplayer modes.

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Running the Application](#running-the-application)
- [Running Tests](#running-tests)
  - [Unit Tests](#unit-tests)
  - [Integration Tests](#integration-tests)
  - [Running All Tests](#running-all-tests)
- [API Endpoints](#api-endpoints)
- [Project Architecture](#project-architecture)

## Project Overview

TicTacToe.Api is a REST API service that implements a complete Tic Tac Toe game engine with the following capabilities:

- **Game Management**: Create, retrieve, and manage multiple game instances
- **Game Modes**: Support for both Two-Player (PvP) and Computer (PvAI) modes
- **Game Operations**: Move validation, undo functionality, turn skipping, and game reset
- **Scoreboard System**: Track player wins, opponent wins, and draws across game sessions
- **Computer AI**: Intelligent computer opponent with winning and blocking strategies
- **Move History**: Complete tracking of all moves made in a game session

## Features

### Core Gameplay
- ✅ 3x3 grid-based Tic Tac Toe implementation
- ✅ Two-player mode (Player X vs Player O)
- ✅ Computer mode (Player vs AI opponent)
- ✅ Win detection (horizontal, vertical, diagonal)
- ✅ Draw detection (board full, no winner)
- ✅ Move undo functionality
- ✅ Skip turn capability
- ✅ Game reset functionality

### Scoreboard Management
- ✅ Track X player wins
- ✅ Track O player wins
- ✅ Track draw games
- ✅ Reset scoreboard

### Computer AI
- ✅ Win-first strategy (completes winning moves)
- ✅ Blocking strategy (blocks opponent wins)
- ✅ Random valid move selection
- ✅ Automatic play after user moves in Computer mode

### Testing
- ✅ 93 comprehensive unit tests
- ✅ 14 integration tests
- ✅ Full API endpoint coverage
- ✅ Game logic validation

## Project Structure

```
TicTacToe.Api/
├── TicTacToe.Api/                          # Main API Project
│   ├── Controllers/
│   │   ├── GamesController.cs              # Game operations endpoints
│   │   └── ScoreboardController.cs         # Scoreboard endpoints
│   ├── Models/
│   │   ├── Game.cs                         # Game domain model
│   │   ├── GameCreateRequest.cs            # Create game request DTO
│   │   ├── GameResponse.cs                 # Game response DTO
│   │   ├── Move.cs                         # Move model
│   │   ├── MoveResponse.cs                 # Move response DTO
│   │   ├── Scoreboard.cs                   # Scoreboard model
│   │   └── Enums/
│   │       ├── GameMode.cs                 # TwoPlayer, Computer
│   │       ├── GameStatus.cs               # InProgress, Won, Draw
│   │       └── Player.cs                   # X, O players
│   ├── Service/
│   │   └── GameService.cs                  # Core game logic service
│   ├── Properties/
│   │   └── launchSettings.json             # Launch configuration
│   ├── Program.cs                          # Application startup
│   ├── appsettings.json                    # App configuration
│   └── TicTacToe.Api.csproj               # Project file
│
├── TicTacToe.Api.Tests/                    # Unit Tests Project
│   ├── GamesControllerTests.cs             # Controller unit tests
│   ├── GameServiceTests.cs                 # Service logic tests
│   ├── GameModelTests.cs                   # Model tests
│   ├── MoveModelTests.cs                   # Move model tests
│   ├── ScoreboardModelTests.cs             # Scoreboard model tests
│   ├── ScoreboardControllerTests.cs        # Scoreboard controller tests
│   ├── ModelsTests.cs                      # General model tests
│   ├── EnumTests.cs                        # Enum validation tests
│   └── TicTacToe.Api.Tests.csproj         # Test project file
│
├── TicTacToe.Api.IntegrationTests/         # Integration Tests Project
│   ├── GamesControllerIntegrationTests.cs  # Games API integration tests
│   ├── ScoreboardControllerIntegrationTests.cs  # Scoreboard API tests
│   ├── HttpContentExtensions.cs            # JSON deserialization helper
│   └── TicTacToe.Api.IntegrationTests.csproj
│
└── TicTacToe.Api.slnx                      # Solution file
```

## Prerequisites

- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/10.0)
- **A text editor or IDE** - Visual Studio Code, Visual Studio, or similar
- **PowerShell or Terminal** - For running dotnet commands

### Verify Installation

```bash
dotnet --version
```

Should output `10.0.x` or higher.

## Getting Started

### 1. Clone or Navigate to Repository

```bash
cd c:\Users\KishanKumarMN\source\repos\TicTacToe.Api
```

### 2. Restore Dependencies

```bash
dotnet restore
```

This downloads all required NuGet packages defined in the project files.

### 3. Build the Solution

```bash
dotnet build
```

This compiles all projects (API, Unit Tests, and Integration Tests).

## Running the Application

### Start the API Server

```bash
dotnet run --project TicTacToe.Api
```

**Output:**
```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

### Access the API

- **Base URL**: `http://localhost:5000` or `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000/swagger` (API documentation)
- **Health Check**: `http://localhost:5000/health` (if configured)

### Example API Call

```bash
# Create a new game (TwoPlayer mode)
curl -X POST "http://localhost:5000/api/games" \
  -H "Content-Type: application/json" \
  -d '{"mode": "TwoPlayer"}'

# Get game status
curl -X GET "http://localhost:5000/api/games/{gameId}"

# Make a move
curl -X POST "http://localhost:5000/api/games/{gameId}/moves" \
  -H "Content-Type: application/json" \
  -d '{"row": 0, "column": 0}'

# Get scoreboard
curl -X GET "http://localhost:5000/api/scoreboard"
```

## Running Tests

### Unit Tests

Unit tests validate individual components (services, controllers, models) in isolation.

**Run all unit tests:**
```bash
dotnet test TicTacToe.Api.Tests
```

**Run specific test class:**
```bash
dotnet test TicTacToe.Api.Tests --filter "GameServiceTests"
```

**Run with verbose output:**
```bash
dotnet test TicTacToe.Api.Tests --verbosity detailed
```

**Expected Output:**
```
Test summary: total: 93, failed: 0, succeeded: 93, skipped: 0, duration: 2.5s
Build succeeded
```

#### Unit Test Coverage

- **GamesControllerTests** (15 tests): Controller endpoints validation
- **GameServiceTests** (44 tests): Game logic, AI, win/draw detection
- **GameModelTests** (19 tests): Game model properties and initialization
- **ScoreboardControllerTests** (7 tests): Scoreboard operations
- **ScoreboardModelTests** (6 tests): Scoreboard model behavior
- **MoveModelTests** (2 tests): Move model validation
- **EnumTests** (7 tests): Enum values verification

### Integration Tests

Integration tests validate the entire API stack with a running application instance using `WebApplicationFactory<Program>`.

**Run all integration tests:**
```bash
dotnet test TicTacToe.Api.IntegrationTests
```

**Run specific test:**
```bash
dotnet test TicTacToe.Api.IntegrationTests --filter "Create_WithTwoPlayerMode_ReturnsOkWithGameResponse"
```

**Run with detailed logging:**
```bash
dotnet test TicTacToe.Api.IntegrationTests --logger "console;verbosity=detailed"
```

**Expected Output:**
```
Test summary: total: 14, failed: 0, succeeded: 14, skipped: 0, duration: 3.2s
Build succeeded
```

#### Integration Test Coverage

**GamesControllerIntegrationTests** (9 tests):
- Create game (TwoPlayer and Computer modes)
- Get game by ID (valid and invalid)
- Make moves and validation
- Reset game state
- Skip turns
- Undo moves

**ScoreboardControllerIntegrationTests** (5 tests):
- Get current scoreboard
- Reset scoreboard
- End-to-end gameplay scenarios
- Score tracking after wins
- Score tracking after draws

### Running All Tests

To run both unit tests and integration tests together:

```bash
dotnet test
```

**Expected Output:**
```
Passed!  - Failed:     0, Passed:    93, Skipped:     0, Total:    93, 
Duration: 245 ms - TicTacToe.Api.Tests.dll (net10.0)

Passed!  - Failed:     0, Passed:    14, Skipped:     0, Total:    14, 
Duration: 657 ms - TicTacToe.Api.IntegrationTests.dll (net10.0)
```

### Test Statistics

| Component | Tests | Status |
|-----------|-------|--------|
| Unit Tests | 93 | ✅ All Passing |
| Integration Tests | 14 | ✅ All Passing |
| **Total** | **107** | **✅ All Passing** |

## API Endpoints

### Games Controller

#### Create Game
```
POST /api/games
Content-Type: application/json

Request Body:
{
  "mode": "TwoPlayer"  // or "Computer"
}

Response:
{
  "id": "uuid",
  "board": [["X", "O", null], ...],
  "currentPlayer": "X",
  "status": "InProgress",
  "mode": "TwoPlayer",
  "winner": null,
  "moves": [],
  "winningCells": []
}
```

#### Get Game
```
GET /api/games/{id}

Response:
{
  "id": "uuid",
  "board": [["X", "O", null], ...],
  "currentPlayer": "O",
  "status": "InProgress",
  "mode": "TwoPlayer",
  "winner": null,
  "moves": [
    {
      "moveNumber": 1,
      "player": "X",
      "row": 0,
      "column": 0
    }
  ],
  "winningCells": []
}
```

#### Make Move
```
POST /api/games/{id}/moves
Content-Type: application/json

Request Body:
{
  "row": 0,
  "column": 1
}

Response:
[Same as Get Game response with updated board]
```

#### Undo Move
```
POST /api/games/{id}/undo
Content-Type: application/json

Response:
[Game state with last move removed]
```

#### Skip Turn
```
POST /api/games/{id}/skip
Content-Type: application/json

Response:
[Game state with player switched]
```

#### Reset Game
```
POST /api/games/{id}/reset
Content-Type: application/json

Response:
[Game state with empty board and InProgress status]
```

### Scoreboard Controller

#### Get Scoreboard
```
GET /api/scoreboard

Response:
{
  "xWins": 5,
  "oWins": 3,
  "draws": 2
}
```

#### Reset Scoreboard
```
POST /api/scoreboard/reset
Content-Type: application/json

Response: 204 No Content
```

## Project Architecture

### Layered Architecture

```
┌─────────────────────────────────────┐
│        HTTP / REST Clients          │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│    Controllers Layer                │
│  - GamesController                  │
│  - ScoreboardController             │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│    Business Logic Layer             │
│  - GameService                      │
│    * Game creation & management     │
│    * Move validation                │
│    * Win/Draw detection             │
│    * Computer AI logic              │
│    * Scoreboard updates             │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│    Domain Models                    │
│  - Game                             │
│  - Move                             │
│  - Scoreboard                       │
│  - Enums (GameMode, Status, Player) │
└─────────────────────────────────────┘
```

### Game Flow

```
1. Create Game
   └─ Initialize 3x3 board
   └─ Set initial player to X
   └─ Set status to InProgress

2. Player Move
   └─ Validate cell is empty
   └─ Validate within bounds
   └─ Validate game not completed
   └─ Update board
   └─ Record move in history
   └─ Check for win/draw
   └─ Switch player

3. Computer Move (Computer Mode)
   └─ Check for winning move
   └─ Check for blocking move
   └─ Select random valid cell
   └─ Repeat Player Move logic

4. Game End
   └─ Detect winner (3 in a row)
   └─ Update scoreboard
   └─ Set status to Won/Draw
   └─ Return final game state
```

### Computer AI Strategy

The computer player uses a three-tier strategy:

1. **Win Strategy**: If a move completes 3 in a row → Take it
2. **Block Strategy**: If opponent can win next turn → Block it
3. **Random Strategy**: Select any available cell randomly

## Building for Production

### Publish Release Build

```bash
dotnet publish -c Release -o ./publish
```

### Run Published Application

```bash
./publish/TicTacToe.Api.exe  # Windows
./publish/TicTacToe.Api      # Linux/macOS
```

## Troubleshooting

### Port Already in Use

If port 5000 or 5001 is already in use:

```bash
dotnet run --project TicTacToe.Api -- --urls="http://localhost:5002"
```

### Tests Failing After Code Changes

Rebuild and clear test results:

```bash
dotnet clean
dotnet build
dotnet test
```

### NuGet Package Issues

Clear package cache and restore:

```bash
dotnet nuget locals all --clear
dotnet restore
```

## Dependencies

### Main Project (TicTacToe.Api)
- ASP.NET Core 10.0
- Swagger/Swashbuckle (API documentation)

### Unit Tests (TicTacToe.Api.Tests)
- xUnit 2.9.3 (Testing framework)
- Moq 4.20.72 (Mocking library)
- Microsoft.NET.Test.Sdk 17.14.1

### Integration Tests (TicTacToe.Api.IntegrationTests)
- Microsoft.AspNetCore.Mvc.Testing 10.0.0
- xUnit 2.9.3

## Contributing

When adding new features:

1. Add corresponding unit tests in `TicTacToe.Api.Tests`
2. Add integration tests if modifying API endpoints in `TicTacToe.Api.IntegrationTests`
3. Ensure all 107 tests pass before committing
4. Follow existing code style and conventions

## License

This project is provided as-is for educational and demonstration purposes.

## Support

For issues, questions, or contributions, please refer to the test files for usage examples and expected behavior.

---

**Last Updated**: June 14, 2026  
**Framework**: .NET 10  
**Status**: ✅ All Tests Passing (107/107)
