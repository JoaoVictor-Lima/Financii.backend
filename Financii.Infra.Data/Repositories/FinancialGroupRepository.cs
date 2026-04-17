using Financii.Domain.Entities;
using Financii.Domain.Enums;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class FinancialGroupRepository : RepositoryBase<FinancialGroup>, IFinancialGroupRepository
    {
        public FinancialGroupRepository(FinanciiDbContext context) : base(context) { }

        public async Task<FinancialGroup?> GetAdminGroupByPersonIdAsync(long personId)
            => await _context.FinancialGroupMembers
                .Where(m => m.PersonId == personId && m.Role == GroupRole.Admin)
                .Join(_context.FinancialGroups,
                    member => member.FinancialGroupId,
                    group => group.Id,
                    (member, group) => group)
                .FirstOrDefaultAsync();
    }
}
