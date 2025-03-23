using LotteryApp.Config;

namespace LotteryApp.UI
{
    public static class UserInterface
    {
        public static void ErrorOccured()
        {
            Console.WriteLine("An error occured. Please restart the application.");
        }

        public static void DisplayWelcomeMessage(
            string playerName)
        {
            Console.WriteLine($"Welcome to the Lottery App, {playerName}.");
        }

        public static void DisplaySelectNumberOfTicketsToPurchase()
        {
            Console.WriteLine("Select number of tickets to purchase...");
        }

        public static void InvalidInput()
        {
            Console.WriteLine("Invalid input. Please try again.");
        }

        public static void ShowPlayerBalance(
            decimal balance)
        {
            Console.WriteLine($"Your current balance is ${balance:F}");
        }

        public static void ShowMessageMaximumNumberOfTicketsAllowed(
            int maximumTicketsPerPlayer)
        {
            Console.WriteLine($"Maximum number of tickets allowed per player is {maximumTicketsPerPlayer}.");
        }

        public static void ShowMessageNumberOfTicketsSelectedBelowMinimumTicketsPerPlayer(
            int minimumTicketAllowed)
        {
            Console.WriteLine($"Minimum number of tickets per player is {minimumTicketAllowed}.");
        }

        public static void ShowMessageBalanceOnlyAllowsNumberOfTickets(
            int allowedNumberOfTickets)
        {
            Console.WriteLine($"Your balance only allows you to buy {allowedNumberOfTickets} ticket(s).");
        }

        public static void ShowMessageNumberOfTicketsPurchased(
            int numberOfTicketsPurchased)
        {
            Console.WriteLine($"You have purchased {numberOfTicketsPurchased} ticket(s).");
        }

        public static void DisplayResults(string results)
        {
            Console.WriteLine(results);
        }

        public static void GameOverMessage()
        {
            Console.WriteLine($"You have insufficent funds to purchase any more tickets, Game Over!");
        }
    }
}