namespace NFLPlayers.Models
{
    public class Player
    {
        public int Number { get; set; }
        
        public string Name { get; set; }

        // The team id could also be a UUID type for production
        public int TeamId { get; set; }

        public int SportId { get; set; }

        public Player()
        {
            Name = string.Empty;
        }

        public Player(int number, string name, int teamId, int sportId)
        {
            Number = number;
            Name = name;
            TeamId = teamId;
            SportId = sportId;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is Player other)
            {
                return Number == other.Number && Name == other.Name && TeamId == other.TeamId && SportId == other.SportId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Name, TeamId, SportId);
        }
    }
}
