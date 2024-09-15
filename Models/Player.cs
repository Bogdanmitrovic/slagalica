namespace Slagalica.Models;

public class Player(string id, string username)
{
    public string Id { get; set; } = id;
    public string Username { get; set; } = username;
}