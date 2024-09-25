namespace Slagalica.Models.DTOs;

public class GameStateDTO
{
    public required string[] PlayerUsernames { get; set; }
    public required int[] Points { get; set; }
    public string[]? Questions { get; set; }
    public string[]? Answers { get; set; }
    public required PlayerAnswerDTO?[] PlayerAnswers { get; set; }
    public required int[] PointsWon { get; set; }
    public string? OnTurn { get; set; }
    public int PlayerCount { get; set; }
    public int Round { get; set; }
    public bool GameEnded { get; set; }
    public bool RoundEnded { get; set; }
    public bool RoomClosing { get; set; }
    public string? CurrentGame { get; set; }
    public int Time { get; set; }
}