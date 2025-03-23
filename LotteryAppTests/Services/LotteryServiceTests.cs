using LotteryApp.Config;
using LotteryApp.Domain;
using LotteryApp.Models;
using LotteryApp.Services;

namespace LotteryAppTests.Services
{
    [TestClass()]
    public class LotteryServiceTests
    {
        [TestMethod()]
        public void AssignTicketsToNonHumanPlayers_Success()
        {
            // Arrange
            var players = PlayerService.GeneratePlayers();

            players.ForEach(p =>
            {
                if (!p.IsHuman)
                {
                    p.Balance = LotteryConfig.MaximumTicketsPerPlayer;
                }
            });

            // Act
            var result = LotteryService.AssignTicketsToNonHumanPlayers(players);

            // Assert
            foreach (var player in result.Where(x => !x.IsHuman))
            {
                Assert.IsTrue(player.NumberOfTickets > 0);
                Assert.IsTrue(player.TicketIds.Count == player.NumberOfTickets);
                Assert.IsTrue(player.Balance < LotteryConfig.MaximumTicketsPerPlayer);
            }
        }

        [TestMethod()]
        public void AssignTicketsToNonHumanPlayers_NoTicketsAssigned()
        {
            // Arrange
            var players = PlayerService.GeneratePlayers();

            players.ForEach(p =>
            {
                if (!p.IsHuman)
                {
                    p.Balance = LotteryConfig.MinimumPlayerBalance;
                }
            });

            // Act
            var result = LotteryService.AssignTicketsToNonHumanPlayers(players);

            // Assert
            foreach (var player in result.Where(x => !x.IsHuman))
            {
                Assert.AreEqual(0, player.NumberOfTickets);
                Assert.AreEqual(0, player.TicketIds.Count);
                Assert.AreEqual(LotteryConfig.MinimumPlayerBalance, player.Balance);
            }
        }

        [TestMethod()]
        public void TryGetHumanPlayer_Success()
        {
            // Arrange
            var players = PlayerService.GeneratePlayers();

            // Act
            var success = LotteryService.TryGetHumanPlayer(players, out Player humanPlayer);

            // Assert
            Assert.IsTrue(success);
            Assert.IsNotNull(humanPlayer);
        }

        [TestMethod()]
        public void TryGetHumanPlayer_Failure()
        {
            // Arrange
            var players = PlayerService.GeneratePlayers();
            players = players.Where(p => !p.IsHuman).ToList();

            // Act
            var success = LotteryService.TryGetHumanPlayer(players, out Player humanPlayer);

            // Assert
            Assert.IsFalse(success);
            Assert.IsNull(humanPlayer);
        }

        [TestMethod()]
        public void TryGetHumanPlayer_ShouldReturnTrue_WhenHumanPlayerExists()
        {
            // Arrange
            var players = new List<Player>
            {
                new () {
                    Id = Guid.NewGuid(),
                    Name = "Computer1",
                    IsHuman = false
                },
                new () {
                    Id = Guid.NewGuid(),
                    Name = "Human",
                    IsHuman = true 
                }
            };

            // Act
            var result = LotteryService.TryGetHumanPlayer(players, out Player humanPlayer);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(humanPlayer);
            Assert.IsTrue(humanPlayer.IsHuman);
        }

        [TestMethod()]
        public void TryGetHumanPlayer_ShouldReturnFalse_WhenHumanPlayerDoesNotExist()
        {
            // Arrange
            var players = new List<Player>
            {
                new () {
                    Id = Guid.NewGuid(),
                    Name = "Computer1",
                    IsHuman = false
                },
                new () { 
                    Id = Guid.NewGuid(),
                    Name = "Computer2", 
                    IsHuman = false
                }
            };

            // Act
            var result = LotteryService.TryGetHumanPlayer(players, out Player humanPlayer);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(humanPlayer);
        }

        [TestMethod()]
        public void AssignTicketsToNonHumanPlayers_ShouldAssignTickets_WhenPlayersHaveSufficientBalance()
        {
            // Arrange
            var players = new List<Player>
            {
                new () {
                    Id = Guid.NewGuid(),
                    Name = "Computer1",
                    IsHuman = false,
                    Balance = 100
                },
                new () {
                    Id = Guid.NewGuid(),
                    Name = "Computer2",
                    IsHuman = false,
                    Balance = 50 
                }
            };

            // Act
            var result = LotteryService.AssignTicketsToNonHumanPlayers(players);

            // Assert
            foreach (var player in result)
            {
                Assert.IsTrue(player.NumberOfTickets > 0);
                Assert.IsTrue(player.TicketIds.Count == player.NumberOfTickets);
                Assert.IsTrue(player.Balance < 100);
            }
        }

        [TestMethod()]
        public void DetermineWinners_ShouldReturnCorrectGameResultModel()
        {
            // Arrange
            var players = new List<Player>
            {
                new () {
                    Id = Guid.NewGuid(),
                    Name = "Player1",
                    IsHuman = true,
                    NumberOfTickets = 5,
                    TicketIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]
                },
                new () {
                    Id = Guid.NewGuid(),
                    Name = "Player2",
                    IsHuman = false,
                    NumberOfTickets = 3,
                    TicketIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]
                }
            };
            int houseProfit = 0;

            // Act
            var result = LotteryService.DetermineWinners(players, houseProfit);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Players.Count == 2);
            Assert.IsTrue(result.HouseProfit >= 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Results));
        }

        [TestMethod()]

        public void GenerateResultsMessage_ShouldReturnCorrectMessage()
        {
            // Arrange
            var players = new List<Player>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Player1",
                    IsHuman = true,
                    NumberOfTickets = 5,
                    DisplayOrder = 1
                },
                new() {
                    Id = Guid.NewGuid(),
                    Name = "Player2",
                    IsHuman = false,
                    NumberOfTickets = 3,
                    DisplayOrder = 2 
                }
            };

            int houseProfit = 100;

            var grandPrizeWinningResult = new WiningResult
            {
                PlayerName = "Player1",
                DisplayOrder = 1,
                NumberOfWiningTickets = 1,
                PrizeAmount = 500
            };

            var secondPrizeWinningResults = new List<WiningResult>
            {
                new() {
                    PlayerName = "Player2",
                    DisplayOrder = 2,
                    NumberOfWiningTickets = 1,
                    PrizeAmount = 200
                }
            };

            var thirdPrizeWinningResults = new List<WiningResult>();

            // Act
            var result = LotteryService.GenerateResultsMessage(players, houseProfit, grandPrizeWinningResult, secondPrizeWinningResults, thirdPrizeWinningResults);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
            Assert.IsTrue(result.Contains("Player1"));
            Assert.IsTrue(result.Contains("Player2"));
            Assert.IsTrue(result.Contains("Total House Revenue: $100"));
        }
    }
}