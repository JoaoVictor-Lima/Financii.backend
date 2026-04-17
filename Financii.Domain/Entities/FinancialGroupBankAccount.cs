using Financii.Domain.Contracts;

namespace Financii.Domain.Entities
{
    public class FinancialGroupBankAccount : EntityBase
    {
        protected FinancialGroupBankAccount() { }

        public FinancialGroupBankAccount(long financialGroupId, long bankAccountId)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            BankAccountId = bankAccountId;
            AddedAt = DateTime.UtcNow;
        }

        public long FinancialGroupId { get; private set; }
        public long BankAccountId { get; private set; }
        public DateTime AddedAt { get; private set; }
    }
}
