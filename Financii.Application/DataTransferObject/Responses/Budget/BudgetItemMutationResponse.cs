namespace Financii.Application.DataTransferObject.Responses.Budget
{
    public class BudgetItemMutationResponse
    {
        public BudgetItemResponse Item { get; set; } = null!;
        public decimal TotalPlanned { get; set; }
    }
}
