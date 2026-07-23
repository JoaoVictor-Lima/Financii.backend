using Financii.Application.DataTransferObject.Responses.Budget;
using Financii.Application.Interfaces.AppServices;
using Financii.Application.Mappers;
using Financii.Infra.Data.Interfaces.Repositories;
using FluentResults;

namespace Financii.Application.AppServices.Budget
{
    public class BudgetCategoryAppService : IBudgetCategoryAppService
    {
        private readonly IBudgetCategoryRepository _repository;

        public BudgetCategoryAppService(IBudgetCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<BudgetCategoryResponse>>> GetAllAsync()
        {
            var categories = await _repository.GetAllOrderedAsync();
            return Result.Ok(BudgetCategoryMapper.ToResponseList(categories));
        }
    }
}
