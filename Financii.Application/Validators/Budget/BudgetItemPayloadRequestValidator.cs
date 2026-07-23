using Financii.Application.DataTransferObject.Requests.Budget;
using FluentValidation;

namespace Financii.Application.Validators.Budget
{
    public class BudgetItemPayloadRequestValidator : AbstractValidator<BudgetItemPayloadRequest>
    {
        public BudgetItemPayloadRequestValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Item description is required.")
                .MaximumLength(100).WithMessage("Item description must be at most 100 characters.");

            RuleFor(x => x.PlannedAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Planned amount must be zero or greater.");

            RuleFor(x => x.CategoryPublicId)
                .NotEmpty().WithMessage("Category is required.");
        }
    }
}
