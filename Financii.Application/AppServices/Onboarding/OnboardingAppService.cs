using Financii.Application.DataTransferObject.Requests.Onboarding;
using Financii.Application.DataTransferObject.Responses.Onboarding;
using Financii.Application.Interfaces.AppServices;
using Financii.Application.Mappers;
using Financii.Domain.Entities;
using Financii.Domain.Enums;
using Financii.Infra.Data.Interfaces.Repositories;
using FluentResults;

namespace Financii.Application.AppServices.Onboarding
{
    public class OnboardingAppService : IOnboardingAppService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IFinancialGroupRepository _financialGroupRepository;
        private readonly IFinancialProfileRepository _financialProfileRepository;
        private readonly IFinancialGroupMemberRepository _financialGroupMemberRepository;
        private readonly IGroupBudgetConfigRepository _groupBudgetConfigRepository;
        private readonly IUnitOfWork _uow;

        public OnboardingAppService(
            IPersonRepository personRepository,
            IFinancialGroupRepository financialGroupRepository,
            IFinancialProfileRepository financialProfileRepository,
            IFinancialGroupMemberRepository financialGroupMemberRepository,
            IGroupBudgetConfigRepository groupBudgetConfigRepository,
            IUnitOfWork uow)
        {
            _personRepository = personRepository;
            _financialGroupRepository = financialGroupRepository;
            _financialProfileRepository = financialProfileRepository;
            _financialGroupMemberRepository = financialGroupMemberRepository;
            _groupBudgetConfigRepository = groupBudgetConfigRepository;
            _uow = uow;
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

        public async Task<Result<CompleteOnboardingResponse>> CompleteAsync(CompleteOnboardingRequest request, long userId)
        {
            var existingPerson = await _personRepository.GetByUserIdAsync(userId);
            if (existingPerson is not null)
                return Result.Fail("ONBOARDING_ALREADY_COMPLETED");

            var detectedProfile = DetectProfile(request);
            CompleteOnboardingResponse response = null!;

            await _uow.ExecuteInTransactionAsync(async () =>
            {
                // Step 1 — Person (needs its DB Id before creating dependents)
                var person = new Person(userId, request.Name, cpf: string.Empty, birthDate: default);
                await _personRepository.AddAsync(person);
                await _uow.CommitAsync();

                // Step 2 — Profile + Group (need group DB Id before creating member)
                var profile = new FinancialProfile(
                    personId: person.Id,
                    detectedProfile: detectedProfile,
                    escudoPercentage: request.EscudoPercentage,
                    hasRecurringIncome: request.HasRecurringIncome,
                    incomeType: request.IncomeType,
                    monthlyIncome: request.MonthlyIncome,
                    hasDebt: request.HasDebt,
                    totalDebtAmount: request.TotalDebtAmount,
                    debtAmountUnknown: request.DebtAmountUnknown,
                    hasEmergencyFund: request.HasEmergencyFund,
                    emergencyFundAmount: request.EmergencyFundAmount);

                var group = new FinancialGroup(name: "Meu Orçamento", createdBy: person.Id);

                await _financialProfileRepository.AddAsync(profile);
                await _financialGroupRepository.AddAsync(group);
                await _uow.CommitAsync();

                // Step 3 — Member + BudgetConfig
                var member = new FinancialGroupMember(group.Id, person.Id, GroupRole.Admin);
                var budgetConfig = new GroupBudgetConfig(
                    financialGroupId: group.Id,
                    escudoPercentage: request.EscudoPercentage,
                    combinedMonthlyIncome: request.MonthlyIncome ?? 0);

                await _financialGroupMemberRepository.AddAsync(member);
                await _groupBudgetConfigRepository.AddAsync(budgetConfig);
                await _uow.CommitAsync();

                response = new CompleteOnboardingResponse
                {
                    FinancialGroupPublicId = group.PublicId,
                    DetectedProfile = OnboardingMapper.ToProfileCode(detectedProfile)
                };
            });

            return Result.Ok(response);
        }

        private static FinancialProfileType DetectProfile(CompleteOnboardingRequest request)
        {
            if (request.HasDebt)
                return FinancialProfileType.Overwhelmed;

            if (!request.HasEmergencyFund)
                return FinancialProfileType.Juggler;

            if (request.MonthlyIncome is null || request.MonthlyIncome <= 6000)
                return FinancialProfileType.Organizer;

            return FinancialProfileType.Household;
        }
    }
}
