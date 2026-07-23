using Financii.Domain.Entities;
using Financii.Domain.Enums;
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
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }

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

            SeedBudgetCategories(builder);
        }

        private static void SeedBudgetCategories(ModelBuilder builder)
        {
            builder.Entity<BudgetCategory>().HasData(new[]
            {
                // Income
                new { Id = 1L, PublicId = new Guid("a1000001-0000-0000-0000-000000000000"), Name = "Salário",         Type = CategoryType.Income,  Icon = (string?)"💰", IsSystem = true },
                new { Id = 2L, PublicId = new Guid("a1000002-0000-0000-0000-000000000000"), Name = "Freelance",       Type = CategoryType.Income,  Icon = (string?)"💼", IsSystem = true },
                new { Id = 3L, PublicId = new Guid("a1000003-0000-0000-0000-000000000000"), Name = "Outras entradas", Type = CategoryType.Income,  Icon = (string?)"➕", IsSystem = true },

                // Expense
                new { Id = 4L,  PublicId = new Guid("a1000004-0000-0000-0000-000000000000"), Name = "Moradia",        Type = CategoryType.Expense, Icon = (string?)"🏠", IsSystem = true },
                new { Id = 5L,  PublicId = new Guid("a1000005-0000-0000-0000-000000000000"), Name = "Alimentação",    Type = CategoryType.Expense, Icon = (string?)"🛒", IsSystem = true },
                new { Id = 6L,  PublicId = new Guid("a1000006-0000-0000-0000-000000000000"), Name = "Transporte",     Type = CategoryType.Expense, Icon = (string?)"🚗", IsSystem = true },
                new { Id = 7L,  PublicId = new Guid("a1000007-0000-0000-0000-000000000000"), Name = "Saúde",          Type = CategoryType.Expense, Icon = (string?)"💊", IsSystem = true },
                new { Id = 8L,  PublicId = new Guid("a1000008-0000-0000-0000-000000000000"), Name = "Educação",       Type = CategoryType.Expense, Icon = (string?)"📚", IsSystem = true },
                new { Id = 9L,  PublicId = new Guid("a1000009-0000-0000-0000-000000000000"), Name = "Lazer",          Type = CategoryType.Expense, Icon = (string?)"🎮", IsSystem = true },
                new { Id = 10L, PublicId = new Guid("a1000010-0000-0000-0000-000000000000"), Name = "Assinaturas",    Type = CategoryType.Expense, Icon = (string?)"📱", IsSystem = true },
                new { Id = 11L, PublicId = new Guid("a1000011-0000-0000-0000-000000000000"), Name = "Vestuário",      Type = CategoryType.Expense, Icon = (string?)"👕", IsSystem = true },
                new { Id = 12L, PublicId = new Guid("a1000012-0000-0000-0000-000000000000"), Name = "Outros gastos",  Type = CategoryType.Expense, Icon = (string?)"📦", IsSystem = true },
            });
        }
    }
}
