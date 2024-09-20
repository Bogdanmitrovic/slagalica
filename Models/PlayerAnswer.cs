namespace Slagalica.Models;

public class PlayerAnswer
{
    public required string PlayerId { get; set; }
    public int Order {get; set;}
    public required string Answer {get; set;}
}