using Financii.Domain.Entities;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IFinancialGroupRepository : IRepositoryBase<FinancialGroup>
    {
        Task<FinancialGroup?> GetAdminGroupByPersonIdAsync(long personId);
    }
}
