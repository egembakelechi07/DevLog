namespace DevLog.Application.DTOs;

// What we return when exposing user data
public sealed class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}


// What we accept when registering a new user
public sealed class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // We accept a plain text password: hashing happens in the handler
    public string Password { get; set; } = string.Empty;
}

// What we accept when a user wants to update their profile
public sealed class UpdateUserDto
{
    public string? Name { get; set; }

    // Email updates should trigger re-verification in production
    public string? Email { get; set; }
}