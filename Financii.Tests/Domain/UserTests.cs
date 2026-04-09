using Financii.Application.DataTransferObject.Requests.User;
using Financii.Application.Validators.User;
using FluentAssertions;
using Xunit;

namespace Financii.Tests.Domain
{
    public class UserTests
    {
        private readonly RegisterUserRequestValidator _validator = new();

        [Fact]
        public void Register_WithValidData_ShouldPassValidation()
        {
            var request = new RegisterUserRequest
            {
                Name = "John Doe",
                Email = "john@email.com",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Register_WithEmptyName_ShouldFail()
        {
            var request = new RegisterUserRequest
            {
                Name = string.Empty,
                Email = "john@email.com",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Name is required.");
        }

        [Fact]
        public void Register_WithInvalidEmail_ShouldFail()
        {
            var request = new RegisterUserRequest
            {
                Name = "John Doe",
                Email = "invalid-email",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Invalid email address.");
        }

        [Fact]
        public void Register_WithShortPassword_ShouldFail()
        {
            var request = new RegisterUserRequest
            {
                Name = "John Doe",
                Email = "john@email.com",
                Password = "123",
                ConfirmPassword = "123"
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Password must be at least 8 characters.");
        }

        [Fact]
        public void Register_WithPasswordMismatch_ShouldFail()
        {
            var request = new RegisterUserRequest
            {
                Name = "John Doe",
                Email = "john@email.com",
                Password = "Password123",
                ConfirmPassword = "Password456"
            };

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Passwords do not match.");
        }
    }
}
