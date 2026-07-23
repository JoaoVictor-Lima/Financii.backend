using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class FinancialGroupMemberRepository : RepositoryBase<FinancialGroupMember>, IFinancialGroupMemberRepository
    {
        public FinancialGroupMemberRepository(FinanciiDbContext context) : base(context) { }

        public async Task<bool> IsPersonInGroupAsync(long personId, long financialGroupId)
            => await Get().AnyAsync(m => m.PersonId == personId && m.FinancialGroupId == financialGroupId);
    }
}
