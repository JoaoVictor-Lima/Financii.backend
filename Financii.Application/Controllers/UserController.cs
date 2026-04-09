using Financii.Application.DataTransferObject.Requests.User;
using Financii.Application.DataTransferObject.Responses;
using Financii.Application.DataTransferObject.Responses.User;
using Financii.Application.Interfaces.AppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserAppService _appService;

        public UserController(IUserAppService appService)
        {
            _appService = appService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterUserRequest request)
            => HandleResult(await _appService.RegisterAsync(request), successStatusCode: 201);

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
            => HandleResult(await _appService.LoginAsync(request));

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Profile()
            => HandleResult(await _appService.GetProfileAsync(GetUserId()));

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateProfile([FromBody] UpdateUserRequest request)
            => HandleResult(await _appService.UpdateProfileAsync(GetUserId(), request));
    }
}
