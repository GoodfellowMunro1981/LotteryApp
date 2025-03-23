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

        public static void DisplaySelectNumberOfTicketsToPurchase(
           string playerName)
        {
            Console.WriteLine("Select number of tickets to purchase...");
        }

        public static void InvalidInput()
        {
            Console.WriteLine("Invalid input. Please try again.");
        }

        public static void ShowPlayerBalance(
            int balance)
        {
            Console.WriteLine($"Your current balance is ${balance}, you can buy {balance} ticket(s).");
        }

        public static void ShowMessageBalanceOnlyAllowsNumberOfTickets(
            int balance)
        {
            Console.WriteLine($"Your balance only allows you to buy {balance} ticket(s), you have purchased {balance} ticket(s)");
        }

        public static void ShowMessageNumberOfTicketsSelectedExceedsMaximumTicketsPerPlayer(
            int maximumTicketsPerPlayer)
        {
            Console.WriteLine($"Maximum number of tickets per player is {maximumTicketsPerPlayer}, you have purchased {maximumTicketsPerPlayer} ticket(s)");
        }

        public static void DisplayResults(string results)
        {
            Console.WriteLine(results);
        }

        public static void GameOverMessage()
        {
            Console.WriteLine($"Your balance is at ${LotteryConfig.MinimumPlayerBalance}, Game Over!");
        }
    }
}