using Financii.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Context
{
    public class FinanciiDbContext : IdentityDbContext<User, Role, long>
    {
        public FinanciiDbContext(DbContextOptions<FinanciiDbContext> options)
            : base(options) { }

        public DbSet<Person> Persons { get; set; }
        public DbSet<FinancialProfile> FinancialProfiles { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankAccountMember> BankAccountMembers { get; set; }
        public DbSet<FinancialGroup> FinancialGroups { get; set; }
        public DbSet<FinancialGroupMember> FinancialGroupMembers { get; set; }
        public DbSet<FinancialGroupBankAccount> FinancialGroupBankAccounts { get; set; }
        public DbSet<GroupBudgetConfig> GroupBudgetConfigs { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Default precision for all decimal properties — avoids silent truncation on SQL Server
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }
        }
    }
}
