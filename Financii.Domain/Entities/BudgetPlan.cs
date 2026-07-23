using Financii.Domain.Contracts;

namespace Financii.Domain.Entities
{
    public class BudgetPlan : EntityBase
    {
        protected BudgetPlan() { }

        public BudgetPlan(long financialGroupId, int month, int year)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            Month = month;
            Year = year;
            TotalPlanned = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public long FinancialGroupId { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public decimal TotalPlanned { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public void RecalculateTotalPlanned(IEnumerable<BudgetItem> items)
            => TotalPlanned = items.Sum(i => i.PlannedAmount);
    }
}
