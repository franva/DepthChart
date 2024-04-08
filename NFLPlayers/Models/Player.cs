namespace NFLPlayers.Models
{
    public class Player
    {
        public int Number { get; set; }
        
        public string Name { get; set; }

        public Player()
        {
            Name = string.Empty;
        }

        public Player(int number, string name)
        {
            Number = number;
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Player other)
            {
                return Number == other.Number && Name == other.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Name);
        }

        public static List<string> CreatePositions(string positions)
        {
            var Positions = new List<string>();
            if (positions.Contains(","))
            {
                foreach (var position in positions.Split(','))
                {
                    if (!string.IsNullOrEmpty(position.Trim()))
                    {
                        Positions.Add(position.Trim());
                    }
                }                
            }
            else
            {
                Positions.Add(positions.Trim());
            }
            return Positions;
        }

    }
}
