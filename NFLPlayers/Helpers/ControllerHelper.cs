using Microsoft.AspNetCore.Http.HttpResults;
using NFLPlayers.Models;
using System.Text.Json;

namespace NFLPlayers.Helpers
{
    public static class ControllerHelper
    {
        public class PlayerWithDepth : Player
        {
            public int? DepthPosition { get; set; }
        }

        public static PlayerWithDepth CreatePlayer(JsonElement request)
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

                return new PlayerWithDepth
                {
                    Number = number,
                    Name = name,
                    Position = position,
                    DepthPosition = positionDepth == int.MinValue ? null : positionDepth
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the player.", ex);
            }
        }
    }
}
