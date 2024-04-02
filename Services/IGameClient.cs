namespace Slagalica.Services;

public interface IGameClient
{
    Task OnJoinGame(string connectionId);
    Task JoinedGame(string roomId);
    Task ReceiveGameState(string state);
    Task OnLeftGame(string contextConnectionId);
    Task LeftGame();
    Task PrepareForGame(int gameIndex);
}