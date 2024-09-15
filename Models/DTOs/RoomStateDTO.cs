namespace Slagalica.Models.DTOs;

public class RoomStateDTO
{
    public List<PlayerDTO> Players { get; set; } = new();
    public required string WhoseTurn { get; set; }
    public int GameType { get; set; }
    public GameStateDTO? GameState { get; set; }
}