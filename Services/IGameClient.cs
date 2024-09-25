using Slagalica.Models.DTOs;

namespace Slagalica.Services;

public interface IGameClient
{
    Task JoinSuccessful(string connectionId);
    Task JoinFailed(string reason);
    Task PlayerJoinedGame(string username);
    Task ReceiveGameState(GameStateDTO state);
    Task OnLeftGame(string contextConnectionId);
    Task LeftGame();
    Task StartTimer(string roomId, int seconds);
    Task StopTimer(string roomId);
    Task GameStartingSoon();
}