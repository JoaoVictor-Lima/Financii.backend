namespace Financii.Application.DataTransferObject.Responses.Budget
{
    public class BudgetSummaryResponse
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalPlanned { get; set; }
        public decimal TotalRealizado { get; set; }
        public double PercentUsed { get; set; }
        public List<BudgetItemSummaryEntry> Items { get; set; } = new();
        public List<UnplannedTransactionEntry> Unplanned { get; set; } = new();
    }

    public class BudgetItemSummaryEntry
    {
        public Guid BudgetItemPublicId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
        public decimal RealizadoAmount { get; set; }
        public double PercentUsed { get; set; }
        public SummaryCategoryInfo Category { get; set; } = null!;
    }

    public class UnplannedTransactionEntry
    {
        public Guid PublicId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public SummaryPersonInfo Person { get; set; } = null!;
        public SummaryCategoryInfo Category { get; set; } = null!;
    }

    public class SummaryCategoryInfo
    {
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
    }

    public class SummaryPersonInfo
    {
        public string Name { get; set; } = string.Empty;
    }
}
