using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;

namespace Financii.Infra.Data.Repositories
{
    public class GroupBudgetConfigRepository : RepositoryBase<GroupBudgetConfig>, IGroupBudgetConfigRepository
    {
        public GroupBudgetConfigRepository(FinanciiDbContext context) : base(context) { }
    }
}
