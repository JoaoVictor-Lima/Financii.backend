using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financii.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFinancialProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasEmergencyReserve",
                table: "FinancialProfiles",
                newName: "HasRecurringIncome");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyIncome",
                table: "FinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "EscudoPercentage",
                table: "FinancialProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FinancialProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "DebtAmountUnknown",
                table: "FinancialProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "EmergencyFundAmount",
                table: "FinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasEmergencyFund",
                table: "FinancialProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IncomeType",
                table: "FinancialProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDebtAmount",
                table: "FinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FinancialProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FinancialProfiles");

            migrationBuilder.DropColumn(
                name: "DebtAmountUnknown",
                table: "FinancialProfiles");

            migrationBuilder.DropColumn(
                name: "EmergencyFundAmount",
                table: "FinancialProfiles");

            migrationBuilder.DropColumn(
                name: "HasEmergencyFund",
                table: "FinancialProfiles");

            migrationBuilder.DropColumn(
                name: "IncomeType",
                table: "FinancialProfiles");

            migrationBuilder.DropColumn(
                name: "TotalDebtAmount",
                table: "FinancialProfiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FinancialProfiles");

            migrationBuilder.RenameColumn(
                name: "HasRecurringIncome",
                table: "FinancialProfiles",
                newName: "HasEmergencyReserve");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyIncome",
                table: "FinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EscudoPercentage",
                table: "FinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
