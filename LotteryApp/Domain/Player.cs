namespace LotteryApp.Domain
{
    public class Player
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int NumberOfTickets { get; set; }

        public List<Guid> TicketIds { get; set; }

        public bool IsHuman { get; set; }

        public int Balance { get; set; }

        public int DisplayOrder { get; set; }
    }
}