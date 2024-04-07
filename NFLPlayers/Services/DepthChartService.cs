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
        
        private bool PlayerNumberExists(int number)
        {
            return _depthCharts.SelectMany(dc => dc.Value).Any(player => player.Number == number);
        }

        private void ValidatePlayer(Player player)
        {
            if (player.Number <= 0)
            {
                throw new ArgumentException("Player number must be a positive non-zero integer.");
            }

            // Check for unique number
            if (PlayerNumberExists(player.Number))
            {
                throw new InvalidOperationException($"A player with number {player.Number} already exists on the depth chart.");
            }

            // Check for non-empty name and position
            if (string.IsNullOrWhiteSpace(player.Name))
            {
                throw new ArgumentException("Player name cannot be null or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(player.Position))
            {
                throw new ArgumentException("Player position cannot be null or whitespace.");
            }
        }

        public void AddPlayerToDepthChart(string position, Player player, int? positionDepth = null)
        {
            ValidatePlayer(player);

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
            if (positionDepth.HasValue && positionDepth.Value >= 0 && positionDepth.Value <= playersAtPosition.Count)
            {
                // Check if the player already exists at this position depth
                if (positionDepth.Value < playersAtPosition.Count && playersAtPosition[positionDepth.Value].Number == player.Number)
                {
                    throw new InvalidOperationException($"A player with number {player.Number} is already at position depth {positionDepth.Value} in position {position}.");
                }

                playersAtPosition.Insert(positionDepth.Value, player);
            }
            else
            {
                if (positionDepth.HasValue && positionDepth.Value > playersAtPosition.Count)
                {
                    throw new ArgumentOutOfRangeException("Position depth is greater than the number of players at this position.");
                }

                // Ensure the player isn't already in the list for this position 
                if (!playersAtPosition.Any(p => p.Number == player.Number))
                {
                    playersAtPosition.Add(player);
                }
            }
        }

        public Player RemovePlayerFromDepthChart(string position, Player player)
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
                ("QB", new Player { Number = 12, Name = "Tom Brady", Position = "QB" }, 0),
                ("QB", new Player { Number = 11, Name = "Blaine Gabbert", Position = "QB" }, 1),
                ("QB", new Player { Number = 2, Name = "Kyle Trask", Position = "QB" }, 2),
                ("WR", new Player { Number = 13, Name = "Mike Evans", Position = "WR" }, 0),
                ("WR", new Player { Number = 14, Name = "Chris Godwin", Position = "WR" }, 1),
                ("RB", new Player { Number = 7, Name = "Leonard Fournette", Position = "RB" }, 0),
                ("RB", new Player { Number = 27, Name = "Ronald Jones II", Position = "RB" }, 1)
            };

            foreach (var (position, player, positionDepth) in players)
            {
                AddPlayerToDepthChart(position, player, positionDepth);
            }
        }

    }

}
