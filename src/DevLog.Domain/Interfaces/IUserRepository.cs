using DevLog.Domain.Entities;

namespace DevLog.Domain.Interfaces;

public interface IUserRepository
{
    // Used during login — find a user by their email
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken= default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresAt);
    bool IsValidPassword(string password, string PasswordHash);
    Task LogoutAsync(Guid userId, CancellationToken cancellationToken);
}