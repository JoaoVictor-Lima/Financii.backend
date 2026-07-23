using Financii.Domain.Enums;

namespace Financii.Application.DataTransferObject.Requests.Transaction
{
    public class CreateTransactionRequest
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public Guid CategoryPublicId { get; set; }
        public Guid? BudgetItemPublicId { get; set; }
        public string? Notes { get; set; }
    }
}
