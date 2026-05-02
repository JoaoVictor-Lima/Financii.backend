using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;

namespace Financii.Infra.Data.Repositories
{
    public class FinancialProfileRepository : RepositoryBase<FinancialProfile>, IFinancialProfileRepository
    {
        public FinancialProfileRepository(FinanciiDbContext context) : base(context) { }
    }
}
