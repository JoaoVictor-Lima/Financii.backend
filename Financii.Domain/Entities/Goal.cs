using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    public class Goal : EntityBase
    {
        protected Goal() { }

        public Goal(
            long financialGroupId,
            string name,
            decimal targetAmount,
            DateTime deadline)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            Name = name;
            TargetAmount = targetAmount;
            CurrentAmount = 0;
            Deadline = deadline;
            Status = GoalStatus.Active;
        }

        public long FinancialGroupId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal TargetAmount { get; private set; }
        public decimal CurrentAmount { get; private set; }
        public DateTime Deadline { get; private set; }
        public GoalStatus Status { get; private set; }
    }
}
