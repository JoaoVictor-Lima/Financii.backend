using Financii.Domain.Contracts;

namespace Financii.Domain.Entities
{
    public class GroupBudgetConfig : EntityBase
    {
        protected GroupBudgetConfig() { }

        public GroupBudgetConfig(
            long financialGroupId,
            decimal escudoPercentage,
            decimal combinedMonthlyIncome)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            EscudoPercentage = escudoPercentage;
            CombinedMonthlyIncome = combinedMonthlyIncome;
            UpdatedAt = DateTime.UtcNow;
        }

        public long FinancialGroupId { get; private set; }
        public decimal EscudoPercentage { get; private set; }
        public decimal CombinedMonthlyIncome { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
