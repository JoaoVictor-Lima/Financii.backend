using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    public class BudgetCategory : EntityBase
    {
        protected BudgetCategory() { }

        public BudgetCategory(string name, CategoryType type, string? icon, bool isSystem)
        {
            PublicId = Guid.NewGuid();
            Name = name;
            Type = type;
            Icon = icon;
            IsSystem = isSystem;
        }

        public string Name { get; private set; } = string.Empty;
        public CategoryType Type { get; private set; }
        public string? Icon { get; private set; }
        public bool IsSystem { get; private set; }
    }
}
