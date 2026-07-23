using Financii.Application.DataTransferObject.Responses.Budget;
using FluentResults;

namespace Financii.Application.Interfaces.AppServices
{
    public interface IBudgetCategoryAppService : IAppService
    {
        Task<Result<List<BudgetCategoryResponse>>> GetAllAsync();
    }
}
