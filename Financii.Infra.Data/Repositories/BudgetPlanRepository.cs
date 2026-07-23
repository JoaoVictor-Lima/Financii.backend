using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class BudgetPlanRepository : RepositoryBase<BudgetPlan>, IBudgetPlanRepository
    {
        public BudgetPlanRepository(FinanciiDbContext context) : base(context) { }

        public async Task<BudgetPlan?> GetByIdAsync(long id)
            => await Get().FirstOrDefaultAsync(p => p.Id == id);

        public async Task<BudgetPlan?> GetByPublicIdAsync(Guid publicId)
            => await Get().FirstOrDefaultAsync(p => p.PublicId == publicId);

        public async Task<BudgetPlan?> GetByGroupMonthYearAsync(long financialGroupId, int month, int year)
            => await Get()
                .FirstOrDefaultAsync(p =>
                    p.FinancialGroupId == financialGroupId &&
                    p.Month == month &&
                    p.Year == year);
    }
}
