using Financii.Domain.Contracts;

namespace Financii.Domain.Entities
{
    public class BudgetItem : EntityBase
    {
        protected BudgetItem() { }

        public BudgetItem(long budgetPlanId, string description, decimal plannedAmount, long categoryId)
        {
            PublicId = Guid.NewGuid();
            BudgetPlanId = budgetPlanId;
            Description = description;
            PlannedAmount = plannedAmount;
            CategoryId = categoryId;
        }

        public long BudgetPlanId { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public decimal PlannedAmount { get; private set; }
        public long CategoryId { get; private set; }

        public void Update(string description, decimal plannedAmount, long categoryId)
        {
            Description = description;
            PlannedAmount = plannedAmount;
            CategoryId = categoryId;
        }
    }
}
