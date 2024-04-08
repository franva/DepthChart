using NFLPlayers.Interfaces;
using NFLPlayers.Models;

namespace NFLPlayers.Services
{
    public class DepthChartService : IDepthChartService
    {
        private readonly Dictionary<string, List<Player>> _depthCharts;

        public DepthChartService()
        {
            _depthCharts = new Dictionary<string, List<Player>>();
        }

        private bool ValidatePlayerInfo(Player player)
        {
            // this mehtod only checks the validity of number and name, if it's valid, return true, otherwise, return false
            if (player.Number <= 0 || string.IsNullOrWhiteSpace(player.Name))
            {
                return false;
            }

            return true;
        }

        public void AddPlayerToDepthChart(string position, Player player, int? positionDepth = null)
        {
            if (string.IsNullOrWhiteSpace(position))
            {
                throw new ArgumentException("Position cannot be null or whitespace.");
            }

            ValidatePlayerInfo(player);

            // Check the positionDepth is valid
            if (positionDepth.HasValue && positionDepth.Value < 0)
            {
                throw new ArgumentOutOfRangeException("Position depth must be a positive integer.");
            }

            if (!_depthCharts.ContainsKey(position))
            {
                _depthCharts[position] = new List<Player>();
            }

            var playersAtPosition = _depthCharts[position];

            // Check if this exact player is already in this exact position
            if (playersAtPosition.Any(p => p.Number == player.Number))
            {
                throw new InvalidOperationException($"A player with number {player.Number} already exists in the {position} position.");
            }
            
            if(positionDepth.HasValue)
            {
                if (positionDepth.Value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(positionDepth), "Position depth cannot be negative.");
                }

                if (positionDepth.Value > playersAtPosition.Count)
                {
                    throw new ArgumentOutOfRangeException("Position depth is greater than the number of players at this position.");
                }

                playersAtPosition.Insert(positionDepth.Value, player);
            } 
            else
            {
                playersAtPosition.Add(player);
            }
        }

        public Player? RemovePlayerFromDepthChart(string position, Player player)
        {
            if (_depthCharts.ContainsKey(position) && _depthCharts[position].Remove(player))
            {
                return player;
            }
            return null;
        }

        public List<Player> GetBackups(string position, Player player)
        {
            List<Player> backups = new List<Player>();
            if (_depthCharts.ContainsKey(position))
            {
                int index = _depthCharts[position].IndexOf(player);
                if (index != -1 && index < _depthCharts[position].Count - 1)
                {
                    backups = _depthCharts[position].GetRange(index + 1, _depthCharts[position].Count - index - 1);
                }
            }
            return backups;
        }

        public Dictionary<string, List<Player>> GetFullDepthChart()
        {
            return new Dictionary<string, List<Player>>(_depthCharts);
        }

        public void SeedData()
        {
            // Example players for seeding the depth chart
            var players = new List<(string position, Player player, int positionDepth)>()
            {
                ("QB", new Player { Number = 12, Name = "Tom Brady" }, 0),
                ("QB", new Player { Number = 11, Name = "Blaine Gabbert" }, 1),
                ("QB", new Player { Number = 2, Name = "Kyle Trask" }, 2),
                ("WR", new Player { Number = 13, Name = "Mike Evans" }, 0),
                ("WR", new Player { Number = 14, Name = "Chris Godwin" }, 1),
                ("RB", new Player { Number = 7, Name = "Leonard Fournette" }, 0),
                ("RB", new Player { Number = 27, Name = "Ronald Jones II" }, 1)
            };

            foreach (var (position, player, positionDepth) in players)
            {
                AddPlayerToDepthChart(position, player, positionDepth);
            }
        }

    }

}
