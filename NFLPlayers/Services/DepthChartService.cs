using NFLPlayers.Interfaces;
using NFLPlayers.Models;

namespace NFLPlayers.Services
{
    public class DepthChartService : IDepthChartService
    {
        private readonly Dictionary<(int SportId, int TeamId, string Position), List<Player>> _depthCharts;

        public DepthChartService()
        {
            _depthCharts = new Dictionary<(int SportId, int TeamId, string Position), List<Player>>();
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

        public void AddPlayerToDepthChart(int sportId, int teamId, string position, Player player, int? positionDepth = null)
        {
            var key = (sportId, teamId, position);

            if (string.IsNullOrWhiteSpace(position))
            {
                throw new ArgumentException("Position cannot be null or whitespace.");
            }

            ValidatePlayerInfo(player);

            if (!_depthCharts.ContainsKey(key))
            {
                _depthCharts[key] = new List<Player>();
            }

            var playersAtPosition = _depthCharts[key];

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

        public Player? RemovePlayerFromDepthChart(int sportId, int teamId, string position, Player player)
        {
            var key = (sportId, teamId, position);

            if (_depthCharts.ContainsKey(key) && _depthCharts[key].Remove(player))
            {
                return player;
            }
            return null;
        }

        public List<Player> GetBackups(int sportId, int teamId, string position, Player player)
        {
            var key = (sportId, teamId, position);
            var playersAtPosition = _depthCharts.GetValueOrDefault(key, new List<Player>());
            int playerIndex = playersAtPosition.IndexOf(player);
            if (playerIndex != -1 && playerIndex < playersAtPosition.Count - 1)
            {
                return playersAtPosition.GetRange(playerIndex + 1, playersAtPosition.Count - playerIndex - 1);
            }
            return new List<Player>();
        }

        public Dictionary<string, List<Player>> GetFullDepthChart(int sportId, int teamId)
        {
            return _depthCharts
                    .Where(k => k.Key.SportId == sportId && k.Key.TeamId == teamId)
                    .ToDictionary(d => d.Key.Position, d => d.Value);
        }

        public void SeedData()
        {
            // Example sports and teams
            int nflId = 1; // NFL Sport ID
            int nbaId = 2; // NBA Sport ID
            int tigersId = 101;
            int lakersId = 201;

            // Example players for seeding the depth chart
            var players = new List<(int sportId, int teamId, string position, Player player, int positionDepth)>()
            {
                // Seeding NFL players
                (nflId, tigersId, "QB", new Player { Number = 12, Name = "Tom Brady" }, 0),
                (nflId, tigersId, "QB", new Player { Number = 11, Name = "Blaine Gabbert" }, 1),
                (nflId, tigersId, "QB", new Player { Number = 2, Name = "Kyle Trask" }, 2),
                (nflId, tigersId, "WR", new Player { Number = 13, Name = "Mike Evans" }, 0),
                (nflId, tigersId, "WR", new Player { Number = 14, Name = "Chris Godwin" }, 1),
                (nflId, tigersId, "RB", new Player { Number = 7, Name = "Leonard Fournette" }, 0),
                (nflId, tigersId, "RB", new Player { Number = 27, Name = "Ronald Jones II" }, 1),

                // Seeding NBA players (example, positions might differ)
                (nbaId, lakersId, "G", new Player { Number = 23, Name = "LeBron James" }, 0),
                (nbaId, lakersId, "G", new Player { Number = 3, Name = "Anthony Davis" }, 1)
            };

            foreach (var (sportId, teamId, position, player, positionDepth) in players)
            {
                AddPlayerToDepthChart(sportId, teamId, position, player, positionDepth);
            }
        }

    }

}
