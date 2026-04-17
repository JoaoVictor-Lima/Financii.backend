using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    public class FinancialGroupMember : EntityBase
    {
        protected FinancialGroupMember() { }

        public FinancialGroupMember(long financialGroupId, long personId, GroupRole role)
        {
            PublicId = Guid.NewGuid();
            FinancialGroupId = financialGroupId;
            PersonId = personId;
            Role = role;
        }

        public long FinancialGroupId { get; private set; }
        public long PersonId { get; private set; }
        public GroupRole Role { get; private set; }
    }
}
