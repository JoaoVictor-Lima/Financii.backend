using Financii.Application.DataTransferObject.Requests.Onboarding;
using Financii.Domain.Enums;
using FluentValidation;

namespace Financii.Application.Validators.Onboarding
{
    public class CompleteOnboardingRequestValidator : AbstractValidator<CompleteOnboardingRequest>
    {
        public CompleteOnboardingRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

            RuleFor(x => x.IncomeType)
                .IsInEnum().WithMessage("Income type is invalid.");

            RuleFor(x => x.MonthlyIncome)
                .NotNull().WithMessage("Monthly income is required when income type is not None.")
                .GreaterThan(0).WithMessage("Monthly income must be positive.")
                .When(x => x.IncomeType != IncomeType.None);

            RuleFor(x => x.MonthlyIncome)
                .Null().WithMessage("Monthly income must be null when income type is None.")
                .When(x => x.IncomeType == IncomeType.None);

            RuleFor(x => x.TotalDebtAmount)
                .NotNull().WithMessage("Total debt amount is required when has debt and amount is not unknown.")
                .When(x => x.HasDebt && !x.DebtAmountUnknown);

            RuleFor(x => x.EscudoPercentage)
                .InclusiveBetween(5, 50).WithMessage("Escudo percentage must be between 5 and 50.")
                .Must(x => x % 5 == 0).WithMessage("Escudo percentage must be a multiple of 5.");
        }
    }
}
