using Slagalica.Models;

namespace Slagalica.Services.Games;

public class LettersStrategy : IGameStrategy
{
    private const int PlayersRoundBonus = 5;
    private const int LongerWordBonus = 5;
    private const int SecondsPerRound = 45;
    private const int SecondsBetweenRounds = 5;
    private static int _questionIndex;
    private static readonly string[][] Questions = [["м", "е", "р", "ш", "з", "а", "т", "т", "з", "б", "и", "а"], ["р", "а", "с", "к", "л", "а", "п", "а", "т", "и", "т", "е"]];
    private static readonly string[][] Answers = [["размештати"],["расклапати"]];
    private static bool LegitWord(string word)
    {
        // TODO check word
        return true;
    }

    private static int CalculateWordScore(string word, string[]? allowedLetters)
    {
        if (!LegitWord(word)) return 0;
        var used = new bool[12];
        foreach (var t in word)
        {
            var found = false;
            for (var j = 0; j < 12 && !found; j++)
            {
                if (used[j] || allowedLetters![j][0] != t) continue;
                used[j] = true;
                found = true;
            }

            if (!found) return 0;
        }
        return word.Length;
    }
    public void CalculatePoints(GameState gameState)
    {
        if (gameState.Questions == null || gameState.Answers == null) return;
        var maxPoints = -1;
        for (var i = 0; i < gameState.PlayerCount; i++)
        {
            if (gameState.PlayerAnswers[i] == null)
                gameState.PointsWon[i] = 0;
            else
                gameState.PointsWon[i] = CalculateWordScore(gameState.PlayerAnswers[i]!.Answer, gameState.Questions);
            if (gameState.PointsWon[i] > maxPoints)
                maxPoints = gameState.PointsWon[i];
        }

        for (var i = 0; i < gameState.PlayerIds.Length; i++)
        {
            var bonusPoints = 0;
            if (gameState.PlayerIds[i] == gameState.OnTurn && gameState.PointsWon[i] == maxPoints)
                bonusPoints += PlayersRoundBonus;
            if (gameState.PointsWon[i] > gameState.Answers[0].Length)
                bonusPoints += LongerWordBonus;
            gameState.PointsWon[i] += bonusPoints;
            gameState.Points[i] += gameState.PointsWon[i];
        }

        if (gameState.Round == gameState.PlayerCount)
        {
            gameState.Time = 5;
            gameState.GameEnded = true;
        }
        else
        {
            gameState.Time = SecondsBetweenRounds;
            gameState.RoundEnded = true;
        }
    }

    public void ReceiveAnswer(GameState gameState, PlayerAnswer playerAnswer)
    {
        gameState.Time = 0; // TODO gde ovo treba da ide
        var i = gameState.PlayerIds.TakeWhile(playerId => playerAnswer.PlayerId != playerId).Count();
        // var order = gameState.PlayerAnswers.Count(answer => answer != null);
        // order++;
        // playerAnswer.Order = order;
        gameState.PlayerAnswers[i] = playerAnswer;
        var playersAnswered = gameState.PlayerAnswers.Count(answer => answer != null);
        if (playersAnswered == gameState.PlayerCount)
            CalculatePoints(gameState);
    }

    public void LoadQuestions(GameState gameState)
    {
        for (var i = 0; i < gameState.PlayerCount; i++)
        {
            gameState.PlayerAnswers[i] = null;
        }

        gameState.Questions = Questions[_questionIndex % Questions.Length];
        gameState.Answers = Answers[_questionIndex%Answers.Length];
        _questionIndex++;
        gameState.Time = SecondsPerRound;
        gameState.Round++;
        gameState.RoundEnded = false;
        gameState.GameEnded = false;
    }
}