using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using Slagalica.Models.DTOs;

namespace Slagalica.Services;

[SignalRHub]
public class GameHub(IGameService gameService, IMapper mapper) : Hub<IGameClient>
{
    public async Task RegisterTimer()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "timer");
        Console.WriteLine($"RegisterTimer: {Context.ConnectionId}");
    }
    
    public async Task JoinRoomById(string username, string roomId)
    {
        if (!gameService.RoomExists(roomId))
        {
            await Clients.Caller.JoinFailed("Room does not exist");
            return;
        }

        if (gameService.RoomIsQuickPlay(roomId))
        {
            await Clients.Caller.JoinFailed("Cannot join quick play room using room number!");
            return;
        }
        
        if (gameService.RoomIsFull(roomId))
        {
            await Clients.Caller.JoinFailed("Room is full!");
            return;
        }

        var success = gameService.JoinRoomById(Context.ConnectionId, username, roomId);
        if (success) await Join(Context.ConnectionId, username, roomId);
        else await Clients.Caller.JoinFailed("Failed to join room!");
    }

    public async Task JoinRoomByPlayer(string username, string otherPlayerId)
    {
        // TODO replace otherPlayerId with otherPlayerUsername and add call to db to find pid from username
        // who should call db?
        var success = gameService.JoinRoomByPlayer(Context.ConnectionId, username, otherPlayerId, out var message);
        if (success)
            await Join(Context.ConnectionId, username, message);
        else
            await Clients.Caller.JoinFailed(message);
    }

    public async Task JoinRoomByLevel(string username, int level)
    {
        var success = gameService.JoinRoomByLevel(Context.ConnectionId, username, level, out var message);
        if (success)
        {
            await Join(Context.ConnectionId, username, message);
            if (await gameService.CanStartGame(message))
                await StartGame(message);
        }
        else
            await Clients.Caller.JoinFailed(message);
    }

    public async Task CreateRoom(string username, int level)
    {
        var roomId = await gameService.CreateRoom(level, false);
        var success = gameService.JoinRoomById(Context.ConnectionId, username, roomId);
        if (success) await Join(Context.ConnectionId, username, roomId);
        else await Clients.Caller.JoinFailed("Failed to create room");
    }

    private async Task Join(string clientId, string username, string roomId)
    {
        await Clients.Group(roomId).PlayerJoinedGame(username);
        await Groups.AddToGroupAsync(clientId, roomId);
        await Clients.Caller.JoinSuccessful(roomId);
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
        if (gameState.Time != 0)
            await Clients.Group("timer").StartTimer(roomId, gameState.Time);
        await Clients.Group(roomId).ReceiveGameState(mapper.Map<GameStateDTO>(gameState));
    }

    public async Task StartGame(string roomId)
    {
        await Clients.Group(roomId).GameStartingSoon();
        await Clients.Group("timer").StartTimer(roomId, 5);
    }
    public async Task TimerOut(string roomId)
    {
        var gameState = await gameService.TimerOutForRoom(roomId);
        if (gameState.Time != 0)
            await Clients.Group("timer").StartTimer(roomId, gameState.Time);
        await Clients.Group(roomId).ReceiveGameState(mapper.Map<GameStateDTO>(gameState));
        if (gameState.RoomClosing)
        {
            await Clients.Group(roomId).LeftGame();
            gameService.CloseRoom(roomId);
        }
    }
    
    public async Task LogServer()
    {
        await gameService.LogServer();
    }
}