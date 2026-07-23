namespace Financii.Application.DataTransferObject.Responses.Budget
{
    public class BudgetCategoryResponse
    {
        public Guid PublicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public bool IsSystem { get; set; }
    }
}
