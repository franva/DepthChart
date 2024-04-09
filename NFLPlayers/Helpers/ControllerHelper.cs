using NFLPlayers.Models;
using System.Text.Json;

namespace NFLPlayers.Helpers
{
    public static class ControllerHelper
    {
        public class PlayerWithExtraInfo
        {
            public Player? Player { get; set; }
            public string? Position { get; set; }
            public int? PositionDepth { get; set; }
        }

        public static PlayerWithExtraInfo CreatePlayer(AddPlayerRequest request)
        {
            try
            {
                int? number = request.Player?.Number;
                string? name = request.Player?.Name;
                string position = request.Position;
                int? positionDepth = request.PositionDepth;
                int? sportId = request?.Player?.SportId;
                int? teamId = request?.Player?.TeamId;

                if (sportId is null || sportId <= 0 || teamId is null || teamId <= 0 || number is null || number <= 0 
                    || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(position))
                {
                    throw new InvalidOperationException("Player data is incomplete or invalid.");
                }

                var player = new Player(number.Value, name, teamId.Value, sportId.Value);
                var playerWithDepth = new PlayerWithExtraInfo
                {
                    Player = player,
                    Position = position,
                    PositionDepth = positionDepth,
                };
                
                return playerWithDepth;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the player.", ex);
            }
        }
        public static PlayerWithExtraInfo CreatePlayer(JsonElement request)
        {
            try
            {
                int number = request.ExtTryGetInt32PropertyCaseInsensitive("number");
                string name = request.ExtTryGetStringPropertyCaseInsensitive("name");
                string position = request.ExtTryGetStringPropertyCaseInsensitive("position");
                int? positionDepth = request.ExtTryGetInt32PropertyCaseInsensitive("positionDepth");
                int? sportId = request.ExtTryGetInt32PropertyCaseInsensitive("sportId");
                int? teamId = request.ExtTryGetInt32PropertyCaseInsensitive("teamId");

                if (sportId is null || sportId < 0 || teamId is null || teamId < 0 || number == int.MinValue 
                    || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(position))
                {
                    throw new InvalidOperationException("Player data is incomplete or invalid.");
                }

                var player = new Player(number, name, teamId.Value, sportId.Value);
                var playerWithDepth = new PlayerWithExtraInfo
                {
                    Player = player,
                    Position = position,
                    PositionDepth = positionDepth
                };
                
                return playerWithDepth;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the player.", ex);
            }
        }
    }
}
