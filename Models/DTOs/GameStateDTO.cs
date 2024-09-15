namespace Slagalica.Models.DTOs;

public class GameStateDTO
{
    public List<string> GameData { get; set; } = new();
    public List<AnswerDTO> Answers { get; set; } = new();
}