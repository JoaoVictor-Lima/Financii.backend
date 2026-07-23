namespace Financii.Application.DataTransferObject.Responses.Budget
{
    public class CategorySummaryResponse
    {
        public Guid PublicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Icon { get; set; }
    }
}
