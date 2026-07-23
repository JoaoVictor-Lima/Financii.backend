using Financii.Domain.Entities;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IBudgetItemRepository : IRepositoryBase<BudgetItem>
    {
        Task<BudgetItem?> GetByPublicIdAsync(Guid publicId);
        Task<List<BudgetItem>> GetByPlanIdAsync(long budgetPlanId);
        Task<List<BudgetItem>> GetByIdsAsync(IEnumerable<long> ids);
    }
}
