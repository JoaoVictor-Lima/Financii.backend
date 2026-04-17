using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    public class BudgetItem : EntityBase
    {
        protected BudgetItem() { }

        public BudgetItem(
            long budgetPlanId,
            string description,
            decimal plannedAmount,
            long categoryId,
            ItemType type,
            bool isRecurring)
        {
            PublicId = Guid.NewGuid();
            BudgetPlanId = budgetPlanId;
            Description = description;
            PlannedAmount = plannedAmount;
            CategoryId = categoryId;
            Type = type;
            IsRecurring = isRecurring;
            IsPaid = false;
        }

        public long BudgetPlanId { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public decimal PlannedAmount { get; private set; }
        public long CategoryId { get; private set; }
        public ItemType Type { get; private set; }
        public bool IsRecurring { get; private set; }
        public bool IsPaid { get; private set; }
    }
}
