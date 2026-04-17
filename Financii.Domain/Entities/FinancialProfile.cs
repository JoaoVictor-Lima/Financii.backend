using Financii.Domain.Contracts;
using Financii.Domain.Enums;

namespace Financii.Domain.Entities
{
    public class FinancialProfile : EntityBase
    {
        protected FinancialProfile() { }

        public FinancialProfile(
            long personId,
            FinancialProfileType detectedProfile,
            int escudoPercentage,
            bool hasRecurringIncome,
            IncomeType incomeType,
            decimal? monthlyIncome,
            bool hasDebt,
            decimal? totalDebtAmount,
            bool debtAmountUnknown,
            bool hasEmergencyFund,
            decimal? emergencyFundAmount)
        {
            PublicId = Guid.NewGuid();
            PersonId = personId;
            DetectedProfile = detectedProfile;
            EscudoPercentage = escudoPercentage;
            HasRecurringIncome = hasRecurringIncome;
            IncomeType = incomeType;
            MonthlyIncome = monthlyIncome;
            HasDebt = hasDebt;
            TotalDebtAmount = totalDebtAmount;
            DebtAmountUnknown = debtAmountUnknown;
            HasEmergencyFund = hasEmergencyFund;
            EmergencyFundAmount = emergencyFundAmount;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public long PersonId { get; private set; }
        public FinancialProfileType DetectedProfile { get; private set; }
        public int EscudoPercentage { get; private set; }

        // Income
        public bool HasRecurringIncome { get; private set; }
        public IncomeType IncomeType { get; private set; }
        public decimal? MonthlyIncome { get; private set; }

        // Debt
        public bool HasDebt { get; private set; }
        public decimal? TotalDebtAmount { get; private set; }
        public bool DebtAmountUnknown { get; private set; }

        // Emergency fund
        public bool HasEmergencyFund { get; private set; }
        public decimal? EmergencyFundAmount { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
