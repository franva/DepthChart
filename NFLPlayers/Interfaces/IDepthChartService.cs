using NFLPlayers.Models;

namespace NFLPlayers.Interfaces
{
    public interface IDepthChartService
    {
        void AddPlayerToDepthChart(string position, Player player, int? positionDepth);
        Player RemovePlayerFromDepthChart(string position, Player player);
        List<Player> GetBackups(string position, Player player);
        Dictionary<string, List<Player>> GetFullDepthChart();

        // This method is used to seed the depth chart with some initial data
        void SeedData();
    }
}
