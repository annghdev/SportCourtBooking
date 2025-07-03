namespace UserService.Entities;

public class User
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateTime? DoB { get; set; }
}
