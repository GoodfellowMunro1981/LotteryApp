using LotteryApp.Config;
using LotteryApp.Domain;

namespace LotteryApp.Services
{
    public static class PlayerService
    {
        private static readonly Random random = new();

        public static List<Player> GeneratePlayers()
        {
            List<Player> players = [];
            int totalPlayers = GetTotalNumberOfPlayers();

            for (int i = 0; i < totalPlayers; i++)
            {
                var playerName = i == 0
                    ? $"Player 1 (Human)"
                    : $"Player {i + 1}";

                var player = new Player
                {
                    Id = Guid.NewGuid(),
                    Name = playerName,
                    NumberOfTickets = 0,
                    TicketIds = [],
                    IsHuman = i == 0,
                    Balance = LotteryConfig.StartingBalance,
                    DisplayOrder = i
                };

                players.Add(player);
            }

            return players;
        }

        public static int GetTotalNumberOfPlayers()
        {
            return random.Next(LotteryConfig.MinimumPlayersRequired, (LotteryConfig.MaximumPlayersAllowed + 1));
        }
    }
}