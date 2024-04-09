using NFLPlayers.Models;

public class AddPlayerRequest
{
    public Player Player { get; set; }
    public string Position { get; set; }
    public int? PositionDepth { get; set; }
}