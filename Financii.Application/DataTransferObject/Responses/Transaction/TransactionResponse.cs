namespace Financii.Application.DataTransferObject.Responses.Transaction
{
    public class TransactionResponse
    {
        public Guid PublicId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public TransactionCategoryInfo Category { get; set; } = null!;
        public Guid? BudgetItemPublicId { get; set; }
        public TransactionPersonInfo Person { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class TransactionCategoryInfo
    {
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
    }

    public class TransactionPersonInfo
    {
        public Guid PublicId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
