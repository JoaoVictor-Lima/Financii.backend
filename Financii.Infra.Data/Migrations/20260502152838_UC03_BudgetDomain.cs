using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Financii.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class UC03_BudgetDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropColumn(
                name: "TotalPlannedExpenses",
                table: "BudgetPlans");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "BudgetItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "BudgetItems");

            migrationBuilder.RenameColumn(
                name: "TotalPlannedIncome",
                table: "BudgetPlans",
                newName: "TotalPlanned");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BudgetPlans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BudgetCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialGroupId = table.Column<long>(type: "bigint", nullable: false),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    BankAccountId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    BudgetItemId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BudgetCategories",
                columns: new[] { "Id", "Icon", "IsSystem", "Name", "PublicId", "Type" },
                values: new object[,]
                {
                    { 1L, "💰", true, "Salário", new Guid("a1000001-0000-0000-0000-000000000000"), 1 },
                    { 2L, "💼", true, "Freelance", new Guid("a1000002-0000-0000-0000-000000000000"), 1 },
                    { 3L, "➕", true, "Outras entradas", new Guid("a1000003-0000-0000-0000-000000000000"), 1 },
                    { 4L, "🏠", true, "Moradia", new Guid("a1000004-0000-0000-0000-000000000000"), 2 },
                    { 5L, "🛒", true, "Alimentação", new Guid("a1000005-0000-0000-0000-000000000000"), 2 },
                    { 6L, "🚗", true, "Transporte", new Guid("a1000006-0000-0000-0000-000000000000"), 2 },
                    { 7L, "💊", true, "Saúde", new Guid("a1000007-0000-0000-0000-000000000000"), 2 },
                    { 8L, "📚", true, "Educação", new Guid("a1000008-0000-0000-0000-000000000000"), 2 },
                    { 9L, "🎮", true, "Lazer", new Guid("a1000009-0000-0000-0000-000000000000"), 2 },
                    { 10L, "📱", true, "Assinaturas", new Guid("a1000010-0000-0000-0000-000000000000"), 2 },
                    { 11L, "👕", true, "Vestuário", new Guid("a1000011-0000-0000-0000-000000000000"), 2 },
                    { 12L, "📦", true, "Outros gastos", new Guid("a1000012-0000-0000-0000-000000000000"), 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetCategories");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BudgetPlans");

            migrationBuilder.RenameColumn(
                name: "TotalPlanned",
                table: "BudgetPlans",
                newName: "TotalPlannedIncome");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPlannedExpenses",
                table: "BudgetPlans",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "BudgetItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "BudgetItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "BudgetItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BankAccountId = table.Column<long>(type: "bigint", nullable: false),
                    BudgetItemId = table.Column<long>(type: "bigint", nullable: true),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinancialGroupId = table.Column<long>(type: "bigint", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                });
        }
    }
}
