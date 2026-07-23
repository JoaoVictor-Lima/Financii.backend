using Financii.Domain.Entities;
using Financii.Domain.Enums;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface ITransactionRepository : IRepositoryBase<Transaction>
    {
        Task<List<Transaction>> GetByGroupAndMonthAsync(long financialGroupId, int month, int year);

        Task<(int totalCount, List<Transaction> items)> ListAsync(
            long financialGroupId,
            int month, int year,
            TransactionType? type,
            long? categoryId,
            long? personId,
            int page, int pageSize);
    }
}
