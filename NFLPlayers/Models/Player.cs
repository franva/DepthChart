using Newtonsoft.Json;

namespace NFLPlayers.Models
{
    public class Player
    {
        [JsonProperty("number")]
        public int Number { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("Position")]
        public string? Position { get; set; }

        public Player()
        {
            Name = string.Empty;
            Position = string.Empty;
        }

        public Player(int number, string name, string? position)
        {
            Number = number;
            Name = name;

            if(position != null)
            {
                Position = position;
            }
            else
            {
                Position = string.Empty;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Player other)
            {
                return Number == other.Number && Name == other.Name && Position == other.Position;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Name, Position);
        }
    }
}
