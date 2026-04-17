using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    public class BankAccountMember : EntityBase
    {
        protected BankAccountMember() { }

        public BankAccountMember(long personId, long bankAccountId, MemberRole role)
        {
            PublicId = Guid.NewGuid();
            PersonId = personId;
            BankAccountId = bankAccountId;
            Role = role;
        }

        public long PersonId { get; private set; }
        public long BankAccountId { get; private set; }
        public MemberRole Role { get; private set; }
    }
}
