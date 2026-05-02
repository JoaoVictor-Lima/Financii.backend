using Financii.Application.DataTransferObject.Requests.Onboarding;
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

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CompleteOnboardingResponse>>> Complete(
            [FromBody] CompleteOnboardingRequest request)
        {
            var result = await _appService.CompleteAsync(request, GetUserId());

            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e.Message == "ONBOARDING_ALREADY_COMPLETED"))
                    return Conflict(new { code = "ONBOARDING_ALREADY_COMPLETED" });

                var errors = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(ApiResponse<CompleteOnboardingResponse>.Failure("Operation not allowed.", errors: errors));
            }

            return StatusCode(201, ApiResponse<CompleteOnboardingResponse>.Success(result.Value, statusCode: 201));
        }
    }
}
