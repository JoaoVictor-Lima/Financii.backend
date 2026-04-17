using Financii.Domain.Contracts;

namespace Financii.Domain.Entities
{
    public class BudgetPlan : EntityBase
    {
        protected BudgetPlan() { }

        public BudgetPlan(
            long financialGroupId,
            int month,
            int year,
            decimal totalPlannedIncome,
            decimal totalPlannedExpenses)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            Month = month;
            Year = year;
            TotalPlannedIncome = totalPlannedIncome;
            TotalPlannedExpenses = totalPlannedExpenses;
        }

        public long FinancialGroupId { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public decimal TotalPlannedIncome { get; private set; }
        public decimal TotalPlannedExpenses { get; private set; }
    }
}
