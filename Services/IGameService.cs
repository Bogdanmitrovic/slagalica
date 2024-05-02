using SignalRSwaggerGen.Attributes;
using Slagalica.Models;

namespace Slagalica.Services;

public interface IGameService
{
    public Task<string> JoinRoom(string playerId, string roomId = "");
    public Task<string?> GetEmptyRoom();
    public Task<string> CreateRoom(string playerId, int maxPlayers = 2);
    public Task StartGame(string roomId);
    public Task<bool> GameStarted(string roomId);
    public Task<string> LeaveGame(string playerId);
    public Task<bool> CanStartGame(string room);
    public Task<GameState> GetRoomState(string roomId);
    public Task<GameState> SendAnswer(string playerId, string answer);
    public Task<string> GetRoomOfPlayer(string contextConnectionId);
}