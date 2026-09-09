namespace BerberVio.API.Models;

public class UserResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string? Email { get; set; }
}
