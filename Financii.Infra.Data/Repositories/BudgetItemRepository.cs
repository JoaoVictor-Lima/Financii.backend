using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class BudgetItemRepository : RepositoryBase<BudgetItem>, IBudgetItemRepository
    {
        public BudgetItemRepository(FinanciiDbContext context) : base(context) { }

        public async Task<BudgetItem?> GetByPublicIdAsync(Guid publicId)
            => await Get().FirstOrDefaultAsync(i => i.PublicId == publicId);

        public async Task<List<BudgetItem>> GetByPlanIdAsync(long budgetPlanId)
            => await Get()
                .Where(i => i.BudgetPlanId == budgetPlanId)
                .ToListAsync();

        public async Task<List<BudgetItem>> GetByIdsAsync(IEnumerable<long> ids)
            => await Get().Where(i => ids.Contains(i.Id)).ToListAsync();
    }
}
