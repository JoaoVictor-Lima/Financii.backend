using Financii.Application.DataTransferObject.Requests.Transaction;
using FluentValidation;

namespace Financii.Application.Validators.Transaction
{
    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(150).WithMessage("Description must be at most 150 characters.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .LessThanOrEqualTo(_ => DateTime.UtcNow.AddYears(1))
                    .WithMessage("Date cannot be more than 1 year in the future.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Transaction type is invalid.");

            RuleFor(x => x.CategoryPublicId)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Notes)
                .MaximumLength(300).WithMessage("Notes must be at most 300 characters.")
                .When(x => x.Notes is not null);
        }
    }
}
