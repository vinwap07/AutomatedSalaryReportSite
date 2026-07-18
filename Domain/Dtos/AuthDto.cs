namespace Domain.Dtos;

public record LoginData
{
    public string Username { get; set; }
    public string Password { get; set; }
}