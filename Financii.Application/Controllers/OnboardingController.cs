using Financii.Application.DataTransferObject.Responses;
using Financii.Application.DataTransferObject.Responses.Onboarding;
using Financii.Application.Interfaces.AppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    [Authorize]
    public class OnboardingController : BaseController
    {
        private readonly IOnboardingAppService _appService;

        public OnboardingController(IOnboardingAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<OnboardingStatusResponse>>> Status()
            => HandleResult(await _appService.GetStatusAsync(GetUserId()));
    }
}
