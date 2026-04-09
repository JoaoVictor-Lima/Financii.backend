using Financii.Application.DataTransferObject.Requests.User;
using Financii.Application.DataTransferObject.Responses.User;
using Financii.Application.Interfaces.AppServices;
using Financii.Application.Interfaces.Repositories;
using FluentResults;

namespace Financii.Application.AppServices.User
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _repository;
        private readonly IJwtService _jwtService;

        public UserAppService(IUserRepository repository, IJwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterUserRequest request)
        {
            var (success, errors, userId) = await _repository.CreateAsync(
                request.Name, request.Email, request.Password);

            if (!success)
                return Result.Fail(errors);

            var user = await _repository.GetByIdAsync(userId);
            return Result.Ok(BuildAuthResponse(user!));
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _repository.ValidateCredentialsAsync(request.Email, request.Password);

            if (user is null)
                return Result.Fail("Invalid email or password.");

            return Result.Ok(BuildAuthResponse(user));
        }

        public async Task<Result<UserResponse>> GetProfileAsync(long userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user is null) return Result.Fail("User not found.");
            return Result.Ok(user);
        }

        public async Task<Result<UserResponse>> UpdateProfileAsync(long userId, UpdateUserRequest request)
        {
            var (success, errors) = await _repository.UpdateAsync(userId, request.Name, request.Email);

            if (!success)
                return Result.Fail(errors);

            return await GetProfileAsync(userId);
        }

        private AuthResponse BuildAuthResponse(UserResponse user) => new()
        {
            Token = _jwtService.GenerateToken(user.Id, user.Email, user.Name),
            ExpiresAt = _jwtService.GetExpiration(),
            User = user
        };
    }
}
