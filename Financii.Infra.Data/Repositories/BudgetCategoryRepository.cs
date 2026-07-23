using Financii.Domain.Entities;
using Financii.Domain.Enums;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class BudgetCategoryRepository : RepositoryBase<BudgetCategory>, IBudgetCategoryRepository
    {
        public BudgetCategoryRepository(FinanciiDbContext context) : base(context) { }

        public async Task<List<BudgetCategory>> GetAllOrderedAsync()
            => await Get()
                .OrderBy(c => c.Type)   // Income (1) before Expense (2)
                .ThenBy(c => c.Name)
                .ToListAsync();
    }
}
