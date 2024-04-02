namespace Slagalica.Models;

public class Room
{
    private int MaxPlayers { get; set; }
    private string RoomId { get; set; }
    private string[] Players { get; set; }
    private int PlayerCount { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsStarted { get; set; }
    public Room(string roomId, int maxPlayers)
    {
        RoomId = roomId;
        MaxPlayers = maxPlayers;
        Players = new string[MaxPlayers];
        PlayerCount = 0;
    }

    public void AddPlayer(string playerId)
    {
        if (PlayerCount == MaxPlayers) throw new Exception("Room is full");
        Players[PlayerCount++] = playerId;
    }
    public void RemovePlayer(string playerId)
    {
        var index = Array.IndexOf(Players, playerId);
        if (index == -1) throw new Exception("Player not found");
        Players[index] = "";
        for (var i = index; i < PlayerCount - 1; i++)
        {
            Players[i] = Players[i + 1];
        }
        PlayerCount--;
    }

    public bool IsFull => PlayerCount == MaxPlayers;
    public bool IsEmpty => PlayerCount == 0;

    public void SendAnswer(string playerId, string answer)
    {
        // TODO ovde se nesto desava sa sobom, poenima i celim game state-om
        Console.WriteLine("ja sam kao uradio nesto");
    }

    public Task<string> GetState()
    {
        return Task.FromResult("neki state");
    }
}