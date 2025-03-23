namespace LotteryApp.Models
{
    public class WiningResult
    {
        public string PlayerName { get; set; }

        public int DisplayOrder { get; set; }

        public int NumberOfWiningTickets { get; set; }

        public decimal PrizeAmount { get; set; }
    }
}