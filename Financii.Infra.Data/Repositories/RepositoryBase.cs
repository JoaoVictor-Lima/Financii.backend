using Financii.Domain.Contracts;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;

namespace Financii.Infra.Data.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly FinanciiDbContext _context;

        public RepositoryBase(FinanciiDbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Get() => _context.Set<TEntity>();

        public async Task AddAsync(TEntity entity)
            => await _context.Set<TEntity>().AddAsync(entity);

        public void Update(TEntity entity)
            => _context.Set<TEntity>().Update(entity);

        public void Delete(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);
    }
}
