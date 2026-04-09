using Financii.Application.Interfaces.Repositories;
using Financii.Infra.Data.Context;

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
