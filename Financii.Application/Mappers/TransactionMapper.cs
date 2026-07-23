using Financii.Application.DataTransferObject.Responses.Transaction;
using Financii.Domain.Entities;

namespace Financii.Application.Mappers
{
    public static class TransactionMapper
    {
        public static TransactionResponse ToResponse(
            Transaction transaction,
            Person person,
            BudgetCategory category,
            Guid? budgetItemPublicId)
            => new()
            {
                PublicId = transaction.PublicId,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Type = transaction.Type.ToString(),
                Category = new TransactionCategoryInfo
                {
                    Name = category.Name,
                    Icon = category.Icon
                },
                BudgetItemPublicId = budgetItemPublicId,
                Person = new TransactionPersonInfo
                {
                    PublicId = person.PublicId,
                    Name = person.Name
                },
                CreatedAt = transaction.CreatedAt
            };
    }
}
