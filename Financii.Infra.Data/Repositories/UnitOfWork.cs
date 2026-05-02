using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;

namespace Financii.Infra.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FinanciiDbContext _context;

        public UnitOfWork(FinanciiDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            // TODO: dispatch Domain Events after commit
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
