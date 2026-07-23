namespace Financii.Application.DataTransferObject.Responses.Transaction
{
    public class PagedTransactionsResponse
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<TransactionListItem> Items { get; set; } = new();
    }

    public class TransactionListItem
    {
        public Guid PublicId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public TransactionCategoryInfo Category { get; set; } = null!;
        public Guid? BudgetItemPublicId { get; set; }
        public TransactionPersonInfo Person { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
