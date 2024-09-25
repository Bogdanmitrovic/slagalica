using SignalRSwaggerGen.Attributes;
using Slagalica.Models;

namespace Slagalica.Services;

public interface IGameService
{
    public bool JoinRoomById(string playerId, string username, string roomId);
    public bool JoinRoomByPlayer(string playerId, string username, string otherPlayerId, out string message);
    public bool JoinRoomByLevel(string playerId, string username, int level, out string message);
    public bool RoomExists(string roomId);
    public bool RoomIsFull(string roomId);
    public Task<string?> GetEmptyRoom();
    public Task<string> CreateRoom(int roomLevel, bool isQuickPlay, int maxPlayers = 2);
    public Task StartGame(string roomId);
    public Task<bool> GameStarted(string roomId);
    public Task<string> LeaveGame(string playerId);
    public Task<bool> CanStartGame(string room);
    public Task<GameState> GetRoomState(string roomId);
    public Task<GameState> SendAnswer(string playerId, string answer);
    public Task<string> GetRoomOfPlayer(string contextConnectionId);
    public Task LogServer();
    public bool RoomIsQuickPlay(string roomId);
    public Task<GameState> TimerOutForRoom(string roomId);
    public void CloseRoom(string roomId);
}