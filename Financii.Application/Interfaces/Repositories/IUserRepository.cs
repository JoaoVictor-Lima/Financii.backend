using Financii.Application.DataTransferObject.Responses.User;

namespace Financii.Application.Interfaces.Repositories
{
    // Does not extend IRepositoryBase — User is managed by Identity (UserManager)
    public interface IUserRepository
    {
        Task<UserResponse?> GetByIdAsync(long id);
        Task<(bool Success, List<string> Errors, long UserId)> CreateAsync(string name, string email, string password);
        Task<(bool Success, List<string> Errors)> UpdateAsync(long id, string name, string email);
        Task<UserResponse?> ValidateCredentialsAsync(string email, string password);
    }
}
