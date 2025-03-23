using System.Data;
using System.Text;
using LotteryApp.Config;
using LotteryApp.Domain;
using LotteryApp.Models;
using LotteryApp.UI;

namespace LotteryApp.Services
{
    public static class LotteryService
    {
        private static readonly Random random = new();

        public static void PlayLotteryDraw(
            List<Player> players)
        {
            if (!TryGetHumanPlayer(players, out Player humanPlayer))
            {
                UserInterface.ErrorOccured();
                return;
            }

            List<Player> computerPlayers = players.Where(p => !p.IsHuman).ToList();
            List<Player> allPlayers = [];
            decimal houseProfit = 0m;

            UserInterface.DisplayWelcomeMessage(humanPlayer.Name);

            bool gameInProgress = false;

            while (humanPlayer.Balance > LotteryConfig.TicketPrice || gameInProgress)
            {
                UserInterface.ShowPlayerBalance(humanPlayer.Balance);
                UserInterface.DisplaySelectNumberOfTicketsToPurchase();

                string? userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    UserInterface.InvalidInput();
                    UserInterface.DisplaySelectNumberOfTicketsToPurchase();
                    continue;
                }

                if (!int.TryParse(userInput, out int requestedNumberOfTickets))
                {
                    UserInterface.InvalidInput();
                    UserInterface.DisplaySelectNumberOfTicketsToPurchase();
                    continue;
                }

                if(requestedNumberOfTickets < LotteryConfig.MinimumTicketsPerPlayer)
                {
                    UserInterface.ShowMessageNumberOfTicketsSelectedBelowMinimumTicketsPerPlayer(LotteryConfig.MinimumTicketsPerPlayer);
                    continue;
                }

                if(requestedNumberOfTickets > LotteryConfig.MaximumTicketsPerPlayer
                    && humanPlayer.Balance >= LotteryConfig.MaximumTicketsPerPlayer)
                {
                    UserInterface.ShowMessageMaximumNumberOfTicketsAllowed(LotteryConfig.MaximumTicketsPerPlayer);
                    requestedNumberOfTickets = LotteryConfig.MaximumTicketsPerPlayer;
                }

                if(requestedNumberOfTickets > humanPlayer.Balance)
                {
                    var allowedNumberOfTickets = (int)Math.Floor(humanPlayer.Balance);
                    UserInterface.ShowMessageBalanceOnlyAllowsNumberOfTickets(allowedNumberOfTickets);
                    requestedNumberOfTickets = allowedNumberOfTickets;
                }

                UserInterface.ShowMessageNumberOfTicketsPurchased(requestedNumberOfTickets);

                humanPlayer.NumberOfTickets = requestedNumberOfTickets;
                humanPlayer.TicketIds = [];

                for (int i = 0; i < humanPlayer.NumberOfTickets; i++)
                {
                    var ticketId = Guid.NewGuid();
                    humanPlayer.TicketIds.Add(ticketId);
                }

                gameInProgress = true;
                humanPlayer.Balance -= requestedNumberOfTickets;
                computerPlayers = AssignTicketsToNonHumanPlayers(computerPlayers);

                allPlayers = computerPlayers.Union([humanPlayer]).ToList();

                var gameResultModel = DetermineWinners(allPlayers, houseProfit);

                allPlayers = [];
                computerPlayers = players.Where(p => !p.IsHuman).ToList();
                TryGetHumanPlayer(players, out humanPlayer);
                houseProfit = gameResultModel.HouseProfit;
                UserInterface.DisplayResults(gameResultModel.Results);
                gameInProgress = false;
            }

            UserInterface.ShowPlayerBalance(humanPlayer.Balance);
            UserInterface.GameOverMessage();
        }

        public static List<Player> AssignTicketsToNonHumanPlayers(
            List<Player> players)
        {
            foreach (var player in players.Where(x => !x.IsHuman))
            {
                if (player.Balance > LotteryConfig.TicketPrice)
                {
                    int playerBalance = (int)Math.Floor(player.Balance);
                    int allowedTickets = Math.Min(playerBalance, LotteryConfig.MaximumTicketsPerPlayer);

                    if(allowedTickets == 0)
                    {
                        continue;
                    }

                    int numberOfTickets = random.Next(LotteryConfig.MinimumTicketsPerPlayer, allowedTickets);

                    player.NumberOfTickets = numberOfTickets;
                    player.Balance -= numberOfTickets * LotteryConfig.TicketPrice;

                    player.TicketIds = [];

                    for (int i = 0; i < player.NumberOfTickets; i++)
                    {
                        var ticketId = Guid.NewGuid();
                        player.TicketIds.Add(ticketId);
                    }
                }
            }

            return players;
        }

        public static GameResultModel DetermineWinners(
            List<Player> players, 
            decimal houseProfit)
        {
            // determine winners
            int totalTickets = players.Sum(p => p.NumberOfTickets);
            decimal totalRevenue = totalTickets * LotteryConfig.TicketPrice;
            decimal grandPrize = totalRevenue * LotteryConfig.FirstPrizePrecentageAsDecimal;

            int secondWinnersCount = (int)Math.Round(totalTickets * LotteryConfig.NumberOfSecondPrizeTicketsPrecentageAsDecimal);
            int thirdWinnersCount = (int)Math.Round(totalTickets * LotteryConfig.NumberOfThirdPrizeTicketsPrecentageAsDecimal);

            List<Guid> allTickets = players.SelectMany(p => p.TicketIds).Distinct().ToList();

            // Grand Prize Winner
            Guid grandPrizeWinningTicket = allTickets[random.Next(allTickets.Count)];


            // Second Prize Winners
            List<Guid> secondPrizeWinningTickets = allTickets
                                                    .Where(x => x != grandPrizeWinningTicket)
                                                    .OrderBy(x => random.Next())
                                                    .Take(secondWinnersCount)
                                                    .ToList();

            // Third Prize Winners
            List<Guid> thirdPrizeWinningTickets = allTickets
                                                    .Where(x => x != grandPrizeWinningTicket
                                                        && !secondPrizeWinningTickets.Contains(x))
                                                    .OrderBy(x => random.Next())
                                                    .Take(thirdWinnersCount)
                                                    .ToList();

            decimal prizeForSecondWinners = (totalRevenue * LotteryConfig.SecondPrizePrecentageAsDecimal) / secondPrizeWinningTickets.Count;
            decimal prizeForThirdWinners = (totalRevenue * LotteryConfig.ThirdPrizePrecentageAsDecimal) / thirdPrizeWinningTickets.Count;


            decimal totalPrizes = grandPrize + (prizeForSecondWinners * secondPrizeWinningTickets.Count) + (prizeForThirdWinners * thirdPrizeWinningTickets.Count);
            decimal gameProfit = totalRevenue - totalPrizes;

            houseProfit += gameProfit;

            var grandPrizeWinningPlayer = players
                                                .Where(p => p.TicketIds.Contains(grandPrizeWinningTicket))
                                                .Select(p => new WiningResult
                                                {
                                                    PlayerName = p.Name,
                                                    DisplayOrder = p.DisplayOrder,
                                                    NumberOfWiningTickets = p.TicketIds.Count(x => grandPrizeWinningTicket == x),
                                                    PrizeAmount = p.TicketIds.Count(x => grandPrizeWinningTicket == x) * grandPrize
                                                })
                                                .FirstOrDefault();

            var secondPrizeWinningPlayers = players
                                                .Where(p => secondPrizeWinningTickets.Any(x => p.TicketIds.Contains(x)))
                                                 .Select(p => new WiningResult
                                                 {
                                                     PlayerName = p.Name,
                                                     DisplayOrder = p.DisplayOrder,
                                                     NumberOfWiningTickets = p.TicketIds.Count(x => secondPrizeWinningTickets.Contains(x)),
                                                     PrizeAmount = p.TicketIds.Count(x => secondPrizeWinningTickets.Contains(x)) * prizeForSecondWinners
                                                 })
                                                .ToList();

            var thirdPrizeWinningPlayers = players
                                                .Where(p => thirdPrizeWinningTickets.Any(x => p.TicketIds.Contains(x)))
                                                .Select(p => new WiningResult
                                                {
                                                    PlayerName = p.Name,
                                                    DisplayOrder = p.DisplayOrder,
                                                    NumberOfWiningTickets = p.TicketIds.Count(x => thirdPrizeWinningTickets.Contains(x)),
                                                    PrizeAmount = p.TicketIds.Count(x => thirdPrizeWinningTickets.Contains(x)) * prizeForThirdWinners
                                                }) 
                                                .ToList();
            foreach (var player in players)
            {
                UpdatePlayersBalanceWithWinnings(player,
                                                    grandPrizeWinningTicket,
                                                    secondPrizeWinningTickets,
                                                    thirdPrizeWinningTickets,
                                                    grandPrize,
                                                    prizeForSecondWinners,
                                                    prizeForThirdWinners);
            }


            var results = GenerateResultsMessage(players,
                                                    houseProfit,
                                                    grandPrizeWinningPlayer,
                                                    secondPrizeWinningPlayers,
                                                    thirdPrizeWinningPlayers);

            return new GameResultModel
            {
                Players = players,
                HouseProfit = houseProfit,
                Results = results
            };
        }

        public static void UpdatePlayersBalanceWithWinnings(
            Player player,
            Guid grandPrizeWinningTicket,
            List<Guid> secondPrizeWinningTickets,
            List<Guid> thirdPrizeWinningTickets,
            decimal grandPrize,
            decimal secondPrizePerWinner,
            decimal thirdPrizePerWinner)
        {
            foreach (var playerTicketId in player.TicketIds)
            {
                if (playerTicketId == grandPrizeWinningTicket)
                {
                    player.Balance += grandPrize;
                    continue;
                }

                if (secondPrizeWinningTickets.Contains(playerTicketId))
                {
                    player.Balance += secondPrizePerWinner;
                    continue;
                }

                if (thirdPrizeWinningTickets.Contains(playerTicketId))
                {
                    player.Balance += thirdPrizePerWinner;
                    continue;
                }
            }
        }

        public static string GenerateResultsMessage(
            List<Player> players,
            decimal houseProfit,
            WiningResult grandPrizeWinningResult,
            List<WiningResult> secondPrizeWinningResults,
            List<WiningResult> thirdPrizeWinningResults)
        {
            var resultBuilder = new StringBuilder();

            resultBuilder.AppendLine("");
            resultBuilder.AppendLine("--- Players and number of ticket(s) purchased ---");

            foreach (var player in players.OrderBy(x => x.DisplayOrder))
            {
                resultBuilder.AppendLine($"{player.Name} - Purchased Tickets: {player.NumberOfTickets}");
            }

            resultBuilder.AppendLine("");
            resultBuilder.AppendLine("--- Winners and Prizes ---");
            resultBuilder.AppendLine($"Grand Prize Winner: {grandPrizeWinningResult.PlayerName} (Number of Winning Tickets {grandPrizeWinningResult.NumberOfWiningTickets}, Prize Total: ${grandPrizeWinningResult.PrizeAmount:F})");

            foreach (var secondPrizeWinningResult in secondPrizeWinningResults.OrderBy(x => x.DisplayOrder))
            {
                resultBuilder.AppendLine($"Second Prize Winners: {secondPrizeWinningResult.PlayerName} (Number of Winning Tickets {secondPrizeWinningResult.NumberOfWiningTickets}, Prize Total: ${secondPrizeWinningResult.PrizeAmount:F})");
            }

            foreach (var thirdPrizeWinningResult in thirdPrizeWinningResults.OrderBy(x => x.DisplayOrder))
            {
                resultBuilder.AppendLine($"Third Prize Winner: {thirdPrizeWinningResult.PlayerName} (Number of Winning Tickets {thirdPrizeWinningResult.NumberOfWiningTickets}, Prize Total: ${thirdPrizeWinningResult.PrizeAmount:F})");
            }

            resultBuilder.AppendLine("");
            resultBuilder.AppendLine($"Total House Revenue: ${houseProfit:F}");
            return resultBuilder.ToString();
        }

        public static bool TryGetHumanPlayer(
            List<Player> players, 
            out Player player)
        {
            var humanPlayer = players.FirstOrDefault(p => p.IsHuman);

            if (humanPlayer == null)
            {
                player = null;
                return false;
            }

            player = humanPlayer;
            return true;
        }
    }
}