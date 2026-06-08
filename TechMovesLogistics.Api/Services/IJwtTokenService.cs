namespace TechMovesLogistics.Api.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username, out DateTime expiresAt);
    }
}
