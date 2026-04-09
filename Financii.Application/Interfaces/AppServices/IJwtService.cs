namespace Financii.Application.Interfaces.AppServices
{
    public interface IJwtService
    {
        string GenerateToken(long userId, string email, string name);
        DateTime GetExpiration();
    }
}
