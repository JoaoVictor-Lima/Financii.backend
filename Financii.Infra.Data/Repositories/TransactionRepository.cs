using Financii.Domain.Entities;
using Financii.Domain.Enums;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(FinanciiDbContext context) : base(context) { }

        public async Task<List<Transaction>> GetByGroupAndMonthAsync(long financialGroupId, int month, int year)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            return await Get()
                .Where(t => t.FinancialGroupId == financialGroupId
                         && t.Date >= start
                         && t.Date < end)
                .ToListAsync();
        }

        public async Task<(int totalCount, List<Transaction> items)> ListAsync(
            long financialGroupId,
            int month, int year,
            TransactionType? type,
            long? categoryId,
            long? personId,
            int page, int pageSize)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            var query = Get()
                .Where(t => t.FinancialGroupId == financialGroupId
                         && t.Date >= start
                         && t.Date < end);

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (personId.HasValue)
                query = query.Where(t => t.PersonId == personId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, items);
        }
    }
}
