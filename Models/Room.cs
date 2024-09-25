using Slagalica.Services.Games;

namespace Slagalica.Models;

public class Room
{
    private int MaxPlayers { get; set; }
    private string RoomId { get; set; }
    private GameState GameState { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsStarted { get; private set; }
    public bool IsQuickPlay { get; set; }
    public int RoomLevel { get; set; }
    private IGameStrategy? CurrentGameStrategy { get; set; }

    public Room(string roomId, int level, int maxPlayers, bool isQuickPlay)
    {
        RoomLevel = level;
        RoomId = roomId;
        MaxPlayers = maxPlayers;
        GameState = new GameState
        {
            PlayerIds = new string[MaxPlayers],
            PlayerUsernames = new string[MaxPlayers],
            Points = new int[MaxPlayers],
            PlayerAnswers = new PlayerAnswer?[MaxPlayers],
            PointsWon = new int[MaxPlayers],
            PlayerCount = 0,
            Round = 0,
            GameEnded = false,
            CurrentGame = "waiting",
            Time = 0
        };
        IsPrivate = !isQuickPlay;
        IsQuickPlay = isQuickPlay;
    }

    public void AddPlayer(string playerId, string username)
    {
        if (GameState.PlayerCount == MaxPlayers) throw new Exception("Room is full");
        GameState.PlayerIds[GameState.PlayerCount] = playerId;
        GameState.PlayerUsernames[GameState.PlayerCount] = username;
        GameState.PlayerCount++;
        //Players[PlayerCount++] = new Player(playerId, username);
    }

    public void RemovePlayer(string playerId)
    {
        var index = Array.IndexOf(GameState.PlayerIds, playerId);
        if (index == -1) throw new Exception("Player not found");
        for (var i = index; i < GameState.PlayerCount - 1; i++)
        {
            GameState.PlayerIds[i] = GameState.PlayerIds[i + 1];
            GameState.PlayerUsernames[i] = GameState.PlayerUsernames[i + 1];
        }

        GameState.PlayerCount--;
        GameState.RoomClosing = GameState.PlayerCount == 0; // TODO is this needed?
    }

    public bool IsFull => GameState.PlayerCount == MaxPlayers;
    public bool IsEmpty => GameState.PlayerCount == 0;
    public bool CanStart => IsQuickPlay ? IsFull : GameState.PlayerCount > 1;

    public GameState SendAnswer(string playerId, string answer)
    {
        Console.WriteLine(playerId + ": " + answer);
        CurrentGameStrategy?.ReceiveAnswer(GameState, new PlayerAnswer { Answer = answer, PlayerId = playerId });
        return GameState;
        //TODO return DTO instead of GameState?
    }

    public GameState GetState()
    {
        return GameState;
    }

    public GameState TimerOut()
    {
        if (IsStarted)
        {
            if (GameState.GameEnded)
            {
                LoadNextGameStrategy();
                CurrentGameStrategy?.LoadQuestions(GameState);
            }
            else if (GameState.RoundEnded)
                CurrentGameStrategy?.LoadQuestions(GameState);
            else
                CurrentGameStrategy?.CalculatePoints(GameState);
        }
        else
        {
            if (GameState.RoomClosing)
            {
                GameState.GameEnded = true;
                GameState.RoundEnded = true;
            }
            else
            {
                IsStarted = true;
                if (CurrentGameStrategy != null && !GameState.GameEnded) return GameState;
                LoadNextGameStrategy();
                CurrentGameStrategy?.LoadQuestions(GameState);
            }
        }
        
        return GameState;
    }

    private void LoadNextGameStrategy()
    {
        switch (GameState.CurrentGame)
        {
            case "waiting":
                CurrentGameStrategy = new LettersStrategy();
                GameState.CurrentGame = "letters";
                break;
            case "letters":
                CurrentGameStrategy = null;
                GameState.CurrentGame = IsQuickPlay ? "finished" : "waiting";
                IsStarted = false;
                GameState.Time = 0;
                GameState.RoomClosing = IsQuickPlay;
                break;
        }
    }
}