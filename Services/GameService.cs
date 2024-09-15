using System.Text;
using Slagalica.Models;

namespace Slagalica.Services;

public class GameService : IGameService
{
    private const int MaxLevelDifference = 2;
    private const string Characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private Dictionary<string, Room> Rooms { get; set; } = new();
    private Dictionary<string, string> PlayerToRoomMap { get; set; } = new();

    public bool JoinRoomById(string playerId, string username, string roomId)
    {
        if (!Rooms.TryGetValue(roomId, out var room)) return false;
        if (room.IsFull) return false;
        room.AddPlayer(playerId, username);
        PlayerToRoomMap[playerId] = roomId;
        return true;
    }

    public bool JoinRoomByPlayer(string playerId, string username, string otherPlayerId, out string message)
    {
        if (!PlayerToRoomMap.TryGetValue(otherPlayerId, out var roomId))
        {
            message = "Player not in room";
            return false;
        }

        var room = Rooms[roomId];
        if (room.IsFull)
        {
            message = "Room is full";
            return false;
        }

        room.AddPlayer(playerId, username);
        PlayerToRoomMap[playerId] = roomId;
        message = roomId;
        return true;
    }

    public bool JoinRoomByLevel(string playerId, string username, int level, out string message)
    {
        var room = Rooms
            .Where(x => !x.Value.IsFull)
            .MinBy(x => Math.Abs(x.Value.RoomLevel - level));
        string roomId;
        if (Rooms.Count == 0 || Math.Abs(room.Value.RoomLevel - level) > MaxLevelDifference)
            roomId = CreateRoom(level).Result;
        else
            roomId = room.Key;
        Rooms[roomId].AddPlayer(playerId, username);
        PlayerToRoomMap[playerId] = roomId;
        message = roomId;
        return true;
    }

    public bool RoomExists(string roomId)
    {
        return Rooms.ContainsKey(roomId);
    }

    public bool RoomIsFull(string roomId)
    {
        return Rooms[roomId].IsFull;
    }

    public Task<string?> GetEmptyRoom()
    {
        var emptyRoom = Rooms.FirstOrDefault(room => room.Value is { IsFull: false, IsPrivate: false });
        return emptyRoom.Key != null ? Task.FromResult<string?>(emptyRoom.Key) : Task.FromResult<string?>(null);
    }

    public Task<string> CreateRoom(int roomLevel, int maxPlayers = 2)
    {
        var newId = GenerateRoomId();
        Rooms.Add(newId, new Room(newId, roomLevel, maxPlayers));
        return Task.FromResult(newId);
    }

    public Task StartGame(string roomId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GameStarted(string roomId)
    {
        return Task.FromResult(Rooms[roomId].IsStarted);
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

    public Task<bool> CanStartGame(string room)
    {
        return Task.FromResult(Rooms[room].CanStart);
    }

    public Task<GameState> GetRoomState(string roomId)
    {
        if (Rooms.TryGetValue(roomId, out var value))
            return value.GetState();
        throw new Exception("Requesting state of non-existing room");
    }

    Task<GameState> IGameService.SendAnswer(string playerId, string answer)
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

    private string GenerateRoomId()
    {
        var random = new Random();
        var result = new StringBuilder();
        while (true)
        {
            for (var i = 0; i < 6; i++)
            {
                result.Append(Characters[random.Next(Characters.Length)]);
            }

            var res = result.ToString();
            if (!Rooms.ContainsKey(res))
                return res;
            result.Clear();
        }
    }
}