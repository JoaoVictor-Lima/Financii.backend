using Financii.Domain.Contracts;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IRepositoryBase<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> Get();
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
