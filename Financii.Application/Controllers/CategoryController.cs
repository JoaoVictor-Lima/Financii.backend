using Financii.Application.DataTransferObject.Responses;
using Financii.Application.DataTransferObject.Responses.Budget;
using Financii.Application.Interfaces.AppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly IBudgetCategoryAppService _appService;

        public CategoryController(IBudgetCategoryAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<BudgetCategoryResponse>>>> GetAll()
            => HandleResult(await _appService.GetAllAsync());
    }
}
