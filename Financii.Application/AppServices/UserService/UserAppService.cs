using Financii.Application.DataTransferObject.Requests.User;
using Financii.Application.DataTransferObject.Responses.User;
using Financii.Application.Interfaces.AppServices;
using Financii.Domain.Entities;
using Financii.Infra.Data.Interfaces.Repositories;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace Financii.Application.AppServices.UserService
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _repository;
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;


        public UserAppService(IUserRepository repository, IJwtService jwtService, UserManager<User> userManager)
        {
            _repository = repository;
            _jwtService = jwtService;
            _userManager = userManager;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterUserRequest request)
        {
            var entity = new User(request.Name, request.Email);

            var result = await _userManager.CreateAsync(entity, request.Password);

            if (!result.Succeeded)
                return Result.Fail(result.Errors.Select(e => e.Description).ToList());

            var user = await _userManager.FindByIdAsync(entity.Id.ToString());

            if (user == null)
            {
                return Result.Fail("TODO"); //TODO: write message
            }

            return Result.Ok(BuildAuthResponse(MapToResponse(user)));
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            var isValid = false;

            if (user != null)
            {
                isValid = await _userManager.CheckPasswordAsync(user, request.Password);
            }

            if (user == null || !isValid)
                return Result.Fail("Invalid email or password.");

            return Result.Ok(BuildAuthResponse(MapToResponse(user)));
        }

        public async Task<Result<UserResponse>> GetProfileAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return Result.Fail("User not found.");
            return Result.Ok(MapToResponse(user));
        }

        public async Task<Result<UserResponse>> UpdateProfileAsync(long userId, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Result.Fail("User not found.");

            user.Name = request.Name;
            user.Email = request.Email;
            user.UserName = request.Email;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Result.Fail(result.Errors.Select(e => e.Description).ToList());

            return await GetProfileAsync(userId);
        }

        private AuthResponse BuildAuthResponse(UserResponse user) => new()
        {
            Token = _jwtService.GenerateToken(user.Id, user.Email, user.Name),
            ExpiresAt = _jwtService.GetExpiration(),
            User = user
        };

        private static UserResponse MapToResponse(User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? string.Empty
        };
    }
}
