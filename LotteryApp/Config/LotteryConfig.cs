namespace LotteryApp.Config
{
    public static class LotteryConfig
    {
        public const int MinimumPlayersRequired = 10;

        public const int MaximumPlayersAllowed = 15;

        public const int StartingBalance = 10; // $10 per player

        public const int TicketPrice = 1;  // $1 per ticket

        public const int MinimumTicketsPerPlayer = 1;

        public const int MaximumTicketsPerPlayer = 10;

        public const int MinimumPlayerBalance = 0;

        public const decimal FirstPrizePrecentageAsDecimal = 0.5m;

        public const decimal SecondPrizePrecentageAsDecimal = 0.3m;

        public const decimal ThirdPrizePrecentageAsDecimal = 0.1m;

        public const decimal NumberOfSecondPrizeTicketsPrecentageAsDecimal = 0.1m;

        public const decimal NumberOfThirdPrizeTicketsPrecentageAsDecimal = 0.1m;
    }
}