using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        public PersonRepository(FinanciiDbContext context) : base(context) { }

        public async Task<Person?> GetByUserIdAsync(long userId)
            => await Get().FirstOrDefaultAsync(p => p.UserId == userId);
    }
}
