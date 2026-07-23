using Financii.Domain.Enums;

namespace Financii.Application.DataTransferObject.Requests.Transaction
{
    public class ListTransactionsQuery
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public TransactionType? Type { get; set; }
        public Guid? CategoryPublicId { get; set; }
        public Guid? PersonPublicId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
