using LotteryApp.Domain;

namespace LotteryApp.Models
{
    public class GameResultModel
    {
        public List<Player> Players { get; set; }

        public int HouseProfit { get; set; }

        public string Results { get; set; }
    }
}