using Financii.Domain.Entities;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IBudgetPlanRepository : IRepositoryBase<BudgetPlan>
    {
        Task<BudgetPlan?> GetByIdAsync(long id);
        Task<BudgetPlan?> GetByPublicIdAsync(Guid publicId);
        Task<BudgetPlan?> GetByGroupMonthYearAsync(long financialGroupId, int month, int year);
    }
}
