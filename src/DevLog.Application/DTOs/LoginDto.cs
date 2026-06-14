namespace DevLog.Application.DTOs;

public class LoginDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
    public string? Scope { get; set; }
    public UserDto User { get; set; } = null!;
}