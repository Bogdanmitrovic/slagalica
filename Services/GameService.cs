using Slagalica.Models;

namespace Slagalica.Services;

public class GameService : IGameService
{
    private Dictionary<string, Room> Rooms { get; set; } = new();
    private Dictionary<string, string> PlayerToRoomMap { get; set; } = new();

    public Task<string> JoinGame(string playerId, string roomId = "")
    {
        if (roomId == "")
        {
            var emptyRoomId = GetEmptyRoom().Result;
            if (emptyRoomId == null) return CreateRoom(playerId);
            Rooms[emptyRoomId].AddPlayer(playerId);
            PlayerToRoomMap[playerId] = emptyRoomId;
            return Task.FromResult(emptyRoomId);
        }

        if (!Rooms.TryGetValue(roomId, out var value)) return CreateRoom(playerId);
        if (value.IsFull) return CreateRoom(playerId);
        Rooms[roomId].AddPlayer(playerId);
        PlayerToRoomMap[playerId] = roomId;
        return Task.FromResult(roomId);
    }

    public Task<string?> GetEmptyRoom()
    {
        var emptyRoom = Rooms.FirstOrDefault(room => room.Value is { IsFull: false, IsPrivate: false });
        return emptyRoom.Key != null ? Task.FromResult<string?>(emptyRoom.Key) : Task.FromResult<string?>(null);
    }

    public Task<string> CreateRoom(string playerId, int maxPlayers = 2)
    {
        var newId = Guid.NewGuid();
        Rooms.Add(newId.ToString(), new Room(newId.ToString(), maxPlayers));
        Rooms[newId.ToString()].AddPlayer(playerId);
        PlayerToRoomMap[playerId] = newId.ToString();
        return Task.FromResult(newId.ToString());
    }

    public Task<string> LeaveGame(string playerId)
    {
        var roomId = PlayerToRoomMap[playerId];
        PlayerToRoomMap.Remove(playerId);
        if (!Rooms.TryGetValue(roomId, out var value)) return Task.FromResult(roomId);
        value.RemovePlayer(playerId);
        if (value.IsEmpty) Rooms.Remove(roomId);
        return Task.FromResult(roomId);
    }

    public Task<bool> ShouldStartGame(string room)
    {
        return Task.FromResult(Rooms[room].IsFull);
    }

    public Task<string> GetRoomState(string roomId)
    {
        if (Rooms.TryGetValue(roomId, out var value))
            return value.GetState();
        throw new Exception("Requesting state of non-existing room");
    }

    Task<string> IGameService.SendAnswer(string playerId, string answer)
    {
        if (!PlayerToRoomMap.TryGetValue(playerId, out var roomId))
            throw new Exception("Player not in room");
        var room = Rooms[roomId];
        room.SendAnswer(playerId, answer);
        return room.GetState();
    }
    public Task<string> GetRoomOfPlayer(string contextConnectionId)
    {
        if (PlayerToRoomMap.TryGetValue(contextConnectionId, out var value))
            return Task.FromResult(value);
        throw new Exception("Player not in room");
    }
}