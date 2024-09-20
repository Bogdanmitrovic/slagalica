using Slagalica.Models;

namespace Slagalica.Services.Games;

public interface IGameStrategy
{
    public void CalculatePoints(GameState gameState);
    public void ReceiveAnswer(GameState gameState, PlayerAnswer playerAnswer);
    public void LoadQuestions(GameState gameState);
}