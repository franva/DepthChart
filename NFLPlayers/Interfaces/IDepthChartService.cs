using NFLPlayers.Models;

namespace NFLPlayers.Interfaces
{
    public interface IDepthChartService
    {
        void AddPlayerToDepthChart(int sportId, int teamId, string position, Player player, int? positionDepth);
        Player? RemovePlayerFromDepthChart(int sportId, int teamId, string position, Player player);
        List<Player> GetBackups(int sportId, int teamId, string position, Player player);
        Dictionary<string, List<Player>> GetFullDepthChart(int sportId, int teamId);

        // This method is used to seed the depth chart with some initial data
        void SeedData();
    }
}
