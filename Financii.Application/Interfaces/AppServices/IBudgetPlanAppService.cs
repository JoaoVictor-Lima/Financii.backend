using Financii.Application.DataTransferObject.Requests.Budget;
using Financii.Application.DataTransferObject.Responses.Budget;
using FluentResults;

namespace Financii.Application.Interfaces.AppServices
{
    public interface IBudgetPlanAppService : IAppService
    {
        Task<Result<CurrentBudgetResponse>> GetCurrentAsync(long userId);
        Task<Result<CurrentBudgetResponse>> CreateAsync(CreateBudgetRequest request, long userId);
        Task<Result<BudgetItemMutationResponse>> AddItemAsync(Guid budgetPublicId, BudgetItemPayloadRequest request, long userId);
        Task<Result<BudgetItemMutationResponse>> UpdateItemAsync(Guid budgetPublicId, Guid itemPublicId, BudgetItemPayloadRequest request, long userId);
        Task<Result<RemoveBudgetItemResponse>> RemoveItemAsync(Guid budgetPublicId, Guid itemPublicId, long userId);
        Task<Result<BudgetSummaryResponse>> GetSummaryAsync(Guid budgetPublicId, long userId);
    }
}
