namespace Financii.Application.DataTransferObject.Responses.Budget
{
    public class BudgetItemResponse
    {
        public Guid PublicId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
        public CategorySummaryResponse Category { get; set; } = null!;
    }
}
