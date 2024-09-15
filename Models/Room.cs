namespace Slagalica.Models;

public class Room
{
    private int MaxPlayers { get; set; }
    private string RoomId { get; set; }
    private Player?[] Players { get; set; }
    private int PlayerCount { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsStarted { get; set; }
    public int RoomLevel { get; set; }

    public Room(string roomId, int level, int maxPlayers)
    {
        RoomLevel = level;
        RoomId = roomId;
        MaxPlayers = maxPlayers;
        Players = new Player?[MaxPlayers];
        PlayerCount = 0;
    }

    public void AddPlayer(string playerId, string username)
    {
        if (PlayerCount == MaxPlayers) throw new Exception("Room is full");
        Players[PlayerCount++] = new Player(playerId, username);
    }

    public void RemovePlayer(string playerId)
    {
        var index = Array.IndexOf(Players, playerId);
        if (index == -1) throw new Exception("Player not found");
        Players[index] = null;
        for (var i = index; i < PlayerCount - 1; i++)
        {
            Players[i] = Players[i + 1];
        }

        PlayerCount--;
    }

    public bool IsFull => PlayerCount == MaxPlayers;
    public bool IsEmpty => PlayerCount == 0;
    public bool CanStart => PlayerCount > 1;

    public bool PlayerCanStartGame(string playerId) => Players.Length>0&& Players[0]?.Id == playerId;

    public void SendAnswer(string playerId, string answer)
    {
        Console.WriteLine("ja sam kao uradio nesto");
    }

    public Task<GameState> GetState()
    {
        return Task.FromResult(new GameState { Time = 10 });
    }
}