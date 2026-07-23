using Financii.Application.DataTransferObject.Requests.Transaction;
using Financii.Application.DataTransferObject.Responses.Transaction;
using FluentResults;

namespace Financii.Application.Interfaces.AppServices
{
    public interface ITransactionAppService : IAppService
    {
        Task<Result<TransactionResponse>> CreateAsync(CreateTransactionRequest request, long userId);
        Task<Result<PagedTransactionsResponse>> ListAsync(ListTransactionsQuery query, long userId);
    }
}
