using DevLog.Domain.Entities;

namespace DevLog.Application.Interfaces;

public interface IUserRepository
{
    // Used during login — find a user by their email
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(string email);
    Task<User> CreateAsync(User user);
}