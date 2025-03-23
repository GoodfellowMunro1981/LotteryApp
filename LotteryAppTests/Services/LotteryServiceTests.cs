using LotteryApp.Config;
using LotteryApp.Domain;
using LotteryApp.Services;

namespace LotteryAppTests.Services
{
    [TestClass()]
    public class LotteryServiceTests
    {
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
    }
}