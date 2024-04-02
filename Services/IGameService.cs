namespace Slagalica.Services;

public interface IGameService
{
    public Task<string> JoinGame(string playerId, string roomId = "");
    public Task<string?> GetEmptyRoom();
    public Task<string> CreateRoom(string playerId, int maxPlayers = 2);
    public Task<string> LeaveGame(string playerId);
    public Task<bool> ShouldStartGame(string room);
    public Task<string> GetRoomState(string roomId);
    public Task<string> SendAnswer(string playerId, string answer);
    public Task<string> GetRoomOfPlayer(string contextConnectionId);
}