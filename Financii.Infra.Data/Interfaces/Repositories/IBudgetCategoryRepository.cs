using Financii.Domain.Entities;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IBudgetCategoryRepository : IRepositoryBase<BudgetCategory>
    {
        Task<List<BudgetCategory>> GetAllOrderedAsync();
    }
}
