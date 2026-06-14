namespace DevLog.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string name, string email);
    string GenerateRefreshToken();
}