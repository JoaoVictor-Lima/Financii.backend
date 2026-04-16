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
            // TODO: disparar Domain Events após commit
        }
    }
}
