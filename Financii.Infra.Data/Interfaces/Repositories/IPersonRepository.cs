using Financii.Domain.Entities;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IPersonRepository : IRepositoryBase<Person>
    {
        Task<Person?> GetByUserIdAsync(long userId);
        Task<Person?> GetByPublicIdAsync(Guid publicId);
        Task<List<Person>> GetByIdsAsync(IEnumerable<long> ids);
    }
}
