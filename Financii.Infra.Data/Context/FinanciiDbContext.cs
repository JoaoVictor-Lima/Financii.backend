using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Context
{
    public class FinanciiDbContext : IdentityDbContext<User, Role, long>
    {
        public FinanciiDbContext(DbContextOptions<FinanciiDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
