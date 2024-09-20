namespace Slagalica.Models;

public class GameState
{
    //public required Dictionary<string,PlayerAnswer> PlayerAnswers { get; set; }
    public required string[] PlayerIds { get; set; }
    public required string[] PlayerUsernames { get; set; }

    public required int[] Points { get; set; }

    // TODO last move needed?
    public string[]? Questions { get; set; }
    public string[]? Answers { get; set; }
    public required PlayerAnswer?[] PlayerAnswers { get; set; }
    public required int[] PointsWon { get; set; }
    public string? OnTurn { get; set; }
    public int PlayerCount { get; set; }
    public int Round { get; set; }
    public bool GameEnded { get; set; }

    public int Time { get; set; }
}