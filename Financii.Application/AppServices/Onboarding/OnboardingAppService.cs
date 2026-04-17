using Financii.Application.DataTransferObject.Responses.Onboarding;
using Financii.Application.Interfaces.AppServices;
using Financii.Infra.Data.Interfaces.Repositories;
using FluentResults;

namespace Financii.Application.AppServices.Onboarding
{
    public class OnboardingAppService : IOnboardingAppService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IFinancialGroupRepository _financialGroupRepository;

        public OnboardingAppService(
            IPersonRepository personRepository,
            IFinancialGroupRepository financialGroupRepository)
        {
            _personRepository = personRepository;
            _financialGroupRepository = financialGroupRepository;
        }

        public async Task<Result<OnboardingStatusResponse>> GetStatusAsync(long userId)
        {
            var person = await _personRepository.GetByUserIdAsync(userId);

            if (person is null)
                return Result.Ok(new OnboardingStatusResponse { Completed = false });

            var group = await _financialGroupRepository.GetAdminGroupByPersonIdAsync(person.Id);

            if (group is null)
                return Result.Ok(new OnboardingStatusResponse { Completed = false });

            return Result.Ok(new OnboardingStatusResponse
            {
                Completed = true,
                FinancialGroupPublicId = group.PublicId
            });
        }
    }
}
