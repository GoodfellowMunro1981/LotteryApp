using LotteryApp.Services;
using LotteryApp.UI;

namespace LotteryApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var players = PlayerService.GeneratePlayers();
                LotteryService.PlayLotteryDraw(players);
            }
            catch (Exception ex)
            {
                // log exception
                UserInterface.ErrorOccured();
                return;
            }
        }
    }
}