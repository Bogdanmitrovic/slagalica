using Microsoft.AspNetCore.SignalR.Client;
using Timer = System.Timers.Timer;

namespace Slagalica.Services;

public class TimerService
{
    private readonly HubConnection _hubConnection;
    private readonly Dictionary<string, int> _timers = new();
    private readonly Timer _timer = new(1000);

    public TimerService(string url, ILogger<TimerService> logger)
    {
        _hubConnection = new HubConnectionBuilder().WithUrl(url).Build();
        _hubConnection.On<string, int>("StartTimer",
            (roomId, seconds) =>
            {
                _timers[roomId] = seconds;
                logger.LogInformation("Timer started in room {RoomId}", roomId);
            });
        _hubConnection.On<string>("StopTimer",
            roomId =>
            {
                if (!_timers.Remove(roomId)) return;
                logger.LogInformation("Timer stopped in room {RoomId}", roomId);
            });
        _hubConnection.Closed += async error =>
        {
            logger.LogError("Connection closed: {Error}", error);
            await _hubConnection.StartAsync();
        };
        _hubConnection.StartAsync();
        _hubConnection.SendAsync("RegisterTimer");
        _timer.Elapsed += (_, _) =>
        {
            foreach (var (roomId, time) in _timers)
            {
                _timers[roomId]--;
                if (time != 0) continue;
                _timers.Remove(roomId);
                _hubConnection.SendAsync("TimerOut", roomId);
                logger.LogInformation("Timer out in room {RoomId}", roomId);
            }
        };
    }
}