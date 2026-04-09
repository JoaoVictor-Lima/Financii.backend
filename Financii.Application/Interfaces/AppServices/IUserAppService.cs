using Financii.Application.DataTransferObject.Requests.User;
using Financii.Application.DataTransferObject.Responses.User;
using FluentResults;

namespace Financii.Application.Interfaces.AppServices
{
    public interface IUserAppService : IAppService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterUserRequest request);
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<UserResponse>> GetProfileAsync(long userId);
        Task<Result<UserResponse>> UpdateProfileAsync(long userId, UpdateUserRequest request);
    }
}
