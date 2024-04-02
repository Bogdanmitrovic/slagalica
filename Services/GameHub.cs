using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Slagalica.Services;

[SignalRHub]
public class GameHub(IGameService gameService) : Hub<IGameClient>
{
    public async Task JoinGame(string roomId = "")
    {
        var room = await gameService.JoinGame(Context.ConnectionId, roomId);
        await Groups.AddToGroupAsync(room, Context.ConnectionId);
        await Clients.GroupExcept(room, Context.ConnectionId).OnJoinGame(Context.ConnectionId);
        await Clients.Caller.JoinedGame(room);
        var shouldStartGame = await gameService.ShouldStartGame(room);
        if (shouldStartGame)
        {
            await Clients.Group(roomId).PrepareForGame(0);
            await Task.Delay(3000);
            await Clients.Group(roomId).ReceiveGameState(gameService.GetRoomState(roomId).Result);
        }
    }
    public async Task LeaveGame()
    {
        var room = await gameService.LeaveGame(Context.ConnectionId);
        await Clients.GroupExcept(room, Context.ConnectionId).OnLeftGame(Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        await Clients.Caller.LeftGame();
    }
    public async Task SendAnswer(string answer)
    {
        var gameState = await gameService.SendAnswer(Context.ConnectionId, answer);
        var roomId = await gameService.GetRoomOfPlayer(Context.ConnectionId);
        await Clients.Group(roomId).ReceiveGameState(gameState);
    }
}