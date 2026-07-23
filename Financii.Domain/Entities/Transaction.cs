using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    public class Transaction : EntityBase
    {
        protected Transaction() { }

        /// <summary>
        /// Manual entry registered by a user (MVP).
        /// personId = financial owner of the transaction (domain identity).
        /// userId = who typed it (manual entry traceability).
        /// </summary>
        public Transaction(
            long financialGroupId,
            long personId,
            long userId,
            string description,
            decimal amount,
            DateTime date,
            TransactionType type,
            long categoryId,
            long? budgetItemId,
            string? notes)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            PersonId = personId;
            UserId = userId;
            Description = description;
            Amount = amount;
            Date = date;
            Type = type;
            CategoryId = categoryId;
            BudgetItemId = budgetItemId;
            Notes = notes;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Automatically imported transaction (Phase 3 — Open Finance).
        /// userId = null because no user typed it.
        /// </summary>
        public Transaction(
            long financialGroupId,
            long personId,
            long bankAccountId,
            string description,
            decimal amount,
            DateTime date,
            TransactionType type,
            long categoryId)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            PersonId = personId;
            UserId = null;
            BankAccountId = bankAccountId;
            Description = description;
            Amount = amount;
            Date = date;
            Type = type;
            CategoryId = categoryId;
            CreatedAt = DateTime.UtcNow;
        }

        public long FinancialGroupId { get; private set; }

        /// <summary>Financial owner of the transaction. Always set.</summary>
        public long PersonId { get; private set; }

        /// <summary>Who manually registered it. Null for automatically imported transactions.</summary>
        public long? UserId { get; private set; }

        /// <summary>Source bank account. Set in Phase 2/3.</summary>
        public long? BankAccountId { get; private set; }

        public string Description { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public TransactionType Type { get; private set; }
        public long CategoryId { get; private set; }
        public long? BudgetItemId { get; private set; }
        public string? Notes { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}
