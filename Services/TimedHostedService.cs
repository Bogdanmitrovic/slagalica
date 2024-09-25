using Microsoft.AspNetCore.SignalR.Client;

namespace Slagalica.Services;

public class TimedHostedService : IHostedService, IDisposable
{
    private readonly ILogger<TimedHostedService> _logger;
    private Timer? _timer;
    private readonly HubConnection _hubConnection;
    private readonly Dictionary<string, int> _timers = new();

    public TimedHostedService(ILogger<TimedHostedService> logger)
    {
        _logger = logger;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5160/gameHub")
            .WithServerTimeout(new TimeSpan(0,0,30))
            .WithAutomaticReconnect()
            .Build();
        // TODO promeni url da nije hardcoded
        _hubConnection.On<string, int>("StartTimer",
            (roomId, seconds) =>
            {
                _timers[roomId] = seconds;
                //logger.LogInformation("Timer started in room {RoomId}", roomId);
                Console.WriteLine($"Timer started in room {roomId}", roomId);
            });
        _hubConnection.On<string>("StopTimer",
            roomId =>
            {
                if (!_timers.Remove(roomId)) return;
                //logger.LogInformation("Timer stopped in room {RoomId}", roomId);
            });
        _hubConnection.Closed += async error =>
        {
            //logger.LogError("Connection closed: {Error}", error);
            await _hubConnection.StartAsync();
        };
        _hubConnection.StartAsync();
        _hubConnection.SendAsync("RegisterTimer");
        //_hubConnection.SendAsync("RegisterTimer");
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(1));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
        {
            _hubConnection.StartAsync();
            _hubConnection.SendAsync("RegisterTimer");
        }
        foreach (var (roomId, time) in _timers)
        {
            _timers[roomId]--;
            Console.WriteLine(roomId + " " + _timers[roomId]);
            if (time != 1) continue;
            _timers.Remove(roomId);
            _hubConnection.SendAsync("TimerOut", roomId);
            //logger.LogInformation("Timer out in room {RoomId}", roomId);
            Console.WriteLine($"Timer out in room {roomId}", roomId);
        }
        //_logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        //_logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}