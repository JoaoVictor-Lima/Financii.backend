using Financii.Domain.Contracts;

namespace Financii.Domain.Entities
{
    public class FinancialGroup : EntityBase
    {
        protected FinancialGroup() { }

        public FinancialGroup(string name, long createdBy)
        {
            PublicId = Guid.NewGuid();
            Name = name;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
        }

        public string Name { get; private set; } = string.Empty;
        public long CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}
