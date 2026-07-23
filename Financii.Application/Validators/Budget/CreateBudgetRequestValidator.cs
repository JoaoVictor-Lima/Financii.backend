using Financii.Application.DataTransferObject.Requests.Budget;
using FluentValidation;

namespace Financii.Application.Validators.Budget
{
    public class CreateBudgetRequestValidator : AbstractValidator<CreateBudgetRequest>
    {
        public CreateBudgetRequestValidator()
        {
            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");

            RuleFor(x => x.Year)
                .InclusiveBetween(2000, 2100).WithMessage("Year must be a valid value.");

            RuleForEach(x => x.Items).SetValidator(new CreateBudgetItemRequestValidator());
        }
    }

    public class CreateBudgetItemRequestValidator : AbstractValidator<CreateBudgetItemRequest>
    {
        public CreateBudgetItemRequestValidator()
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
