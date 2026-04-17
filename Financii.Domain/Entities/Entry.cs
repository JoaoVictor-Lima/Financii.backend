using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    // Renamed from "Lancamento" (PT) → "Entry" (EN)
    public class Entry : EntityBase
    {
        protected Entry() { }

        public Entry(
            long financialGroupId,
            long userId,
            string description,
            decimal amount,
            DateTime date,
            long categoryId,
            ItemType type,
            bool isRecurring,
            long bankAccountId,
            long? budgetItemId = null)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            UserId = userId;
            Description = description;
            Amount = amount;
            Date = date;
            CategoryId = categoryId;
            Type = type;
            IsRecurring = isRecurring;
            BankAccountId = bankAccountId;
            BudgetItemId = budgetItemId;
        }

        public long FinancialGroupId { get; private set; }
        public long UserId { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public long CategoryId { get; private set; }
        public ItemType Type { get; private set; }
        public bool IsRecurring { get; private set; }
        public long BankAccountId { get; private set; }
        public long? BudgetItemId { get; private set; }
    }
}
