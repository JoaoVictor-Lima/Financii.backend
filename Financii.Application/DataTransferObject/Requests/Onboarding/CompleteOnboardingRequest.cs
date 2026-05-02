using Financii.Domain.Enums;

namespace Financii.Application.DataTransferObject.Requests.Onboarding
{
    public class CompleteOnboardingRequest
    {
        public string Name { get; set; } = string.Empty;

        // Income
        public bool HasRecurringIncome { get; set; }
        public IncomeType IncomeType { get; set; }
        public decimal? MonthlyIncome { get; set; }

        // Debt
        public bool HasDebt { get; set; }
        public decimal? TotalDebtAmount { get; set; }
        public bool DebtAmountUnknown { get; set; }

        // Emergency fund
        public bool HasEmergencyFund { get; set; }
        public decimal? EmergencyFundAmount { get; set; }

        public int EscudoPercentage { get; set; }
    }
}
