using DevLog.Domain.Interfaces;
using DevLog.Infrastructure.Data;
using DevLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevLog.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken= default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken );
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresAt)
    {
        var user = await _context.Users.FindAsync(userId);
        user!.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = expiresAt;
        user!.LastUpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return user;
    }

    public bool IsValidPassword(string password, string PasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(userId, cancellationToken);

        if(user == null)
        return;

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        user.LastUpdatedAt = DateTime.UtcNow;

       await _context.SaveChangesAsync(cancellationToken);
    }
}