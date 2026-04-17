using Financii.Application.DataTransferObject.Responses.Onboarding;
using FluentResults;

namespace Financii.Application.Interfaces.AppServices
{
    public interface IOnboardingAppService : IAppService
    {
        Task<Result<OnboardingStatusResponse>> GetStatusAsync(long userId);
    }
}
