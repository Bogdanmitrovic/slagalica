using Slagalica.Models;

namespace Slagalica.Services;

public interface IGameClient
{
    Task OnJoinGame(string connectionId);
    Task JoinedGame(string roomId);
    Task ReceiveGameState(GameState state);
    Task OnLeftGame(string contextConnectionId);
    Task LeftGame();
    Task PrepareForGame(int gameIndex);
    Task StartTimer(string roomId, int seconds);
    Task StopTimer(string roomId);
}