using NFLPlayers.Models;
using System.Text.Json;

namespace NFLPlayers.Helpers
{
    public static class ControllerHelper
    {
        public class PlayerWithPositionDepth
        {
            public Player? Player { get; set; }
            public string? Position { get; set; }
            public int? PositionDepth { get; set; }
        }

        public static PlayerWithPositionDepth CreatePlayer(JsonElement request)
        {
            try
            {
                int number = request.ExtTryGetInt32PropertyCaseInsensitive("number");
                string name = request.ExtTryGetStringPropertyCaseInsensitive("name");
                string position = request.ExtTryGetStringPropertyCaseInsensitive("position");
                int? positionDepth = request.ExtTryGetInt32PropertyCaseInsensitive("positionDepth");

                if (number == int.MinValue || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(position))
                {
                    throw new InvalidOperationException("Player data is incomplete.");
                }

                var player = new Player(number, name);
                var playerWithDepth = new PlayerWithPositionDepth
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
