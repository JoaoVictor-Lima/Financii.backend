namespace Financii.Application.DataTransferObject.Requests.Budget
{
    public class CreateBudgetRequest
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public List<CreateBudgetItemRequest> Items { get; set; } = new();
    }
}
