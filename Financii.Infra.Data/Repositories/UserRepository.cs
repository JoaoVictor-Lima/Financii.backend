using Financii.Application.DataTransferObject.Responses.User;
using Financii.Application.Interfaces.Repositories;
using Financii.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Financii.Infra.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserResponse?> GetByIdAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user is null ? null : MapToResponse(user);
        }

        public async Task<(bool Success, List<string> Errors, long UserId)> CreateAsync(
            string name, string email, string password)
        {
            var user = new User { Name = name, Email = email, UserName = email };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToList(), 0);

            return (true, new List<string>(), user.Id);
        }

        public async Task<(bool Success, List<string> Errors)> UpdateAsync(
            long id, string name, string email)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null)
                return (false, new List<string> { "User not found." });

            user.Name = name;
            user.Email = email;
            user.UserName = email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToList());

            return (true, new List<string>());
        }

        public async Task<UserResponse?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return null;

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            return isValid ? MapToResponse(user) : null;
        }

        private static UserResponse MapToResponse(User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? string.Empty
        };
    }
}
