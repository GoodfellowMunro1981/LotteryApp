using LotteryApp.Config;

namespace LotteryApp.Services.Tests
{
    [TestClass()]
    public class PlayerServiceTests
    {
        [TestMethod()]
        public void GetTotalNumberOfPlayersTest_100Times()
        {
            for(int i = 0; i < 100; i++)
            {
                // Arrange

                // Act
                var totalNumberOfPlayers = PlayerService.GetTotalNumberOfPlayers();

                // Assert
                Assert.IsTrue(totalNumberOfPlayers >= LotteryConfig.MinimumPlayersRequired);
                Assert.IsTrue(totalNumberOfPlayers <= LotteryConfig.MaximumPlayersAllowed);
            }
        }

        [TestMethod()]
        public void GeneratePlayers_100Times()
        {
            for (int i = 0; i < 100; i++)
            {
                // Arrange

                // Act
                var players = PlayerService.GeneratePlayers();

                // Assert
                Assert.IsNotNull(players);
                Assert.IsTrue(players.Count >= LotteryConfig.MinimumPlayersRequired);
                Assert.IsTrue(players.Count <= LotteryConfig.MaximumPlayersAllowed);

                foreach (var player in players)
                {
                    Assert.IsNotNull(player.Id);
                    Assert.IsNotNull(player.Name);
                    Assert.IsTrue(player.NumberOfTickets == 0);
                    Assert.IsNotNull(player.TicketIds);
                    Assert.IsTrue(player.TicketIds.Count == 0);
                    Assert.IsTrue(player.Balance == LotteryConfig.StartingBalance);
                    Assert.IsTrue(player.DisplayOrder >= 0);
                }
            }
        }
    }
}