using Slagalica.Models;

namespace Slagalica.Services;

public interface IGameClient
{
    Task JoinSuccessful(string connectionId);
    Task JoinFailed(string reason);
    Task PlayerJoinedGame(string username);
    Task ReceiveGameState(GameState state);
    Task OnLeftGame(string contextConnectionId);
    Task LeftGame();
    Task PrepareForGame(int gameIndex);
    Task StartTimer(string roomId, int seconds);
    Task StopTimer(string roomId);
    Task LogServer();
    Task GameStartingSoon();
}