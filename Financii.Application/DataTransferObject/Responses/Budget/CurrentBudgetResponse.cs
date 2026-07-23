namespace Financii.Application.DataTransferObject.Responses.Budget
{
    public class CurrentBudgetResponse
    {
        public bool Exists { get; set; }
        public Guid? PublicId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public decimal? TotalPlanned { get; set; }
        public List<BudgetItemResponse>? Items { get; set; }
    }
}
