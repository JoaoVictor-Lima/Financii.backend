namespace Financii.Application.DataTransferObject.Requests.Budget
{
    public class CreateBudgetItemRequest
    {
        public string Description { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
        public Guid CategoryPublicId { get; set; }
    }
}
