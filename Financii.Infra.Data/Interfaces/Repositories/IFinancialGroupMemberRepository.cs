using Financii.Domain.Entities;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IFinancialGroupMemberRepository : IRepositoryBase<FinancialGroupMember>
    {
        Task<bool> IsPersonInGroupAsync(long personId, long financialGroupId);
    }
}
