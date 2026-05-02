using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;

namespace Financii.Infra.Data.Repositories
{
    public class FinancialGroupMemberRepository : RepositoryBase<FinancialGroupMember>, IFinancialGroupMemberRepository
    {
        public FinancialGroupMemberRepository(FinanciiDbContext context) : base(context) { }
    }
}
