using Financii.Application.DataTransferObject.Requests.Budget;
using Financii.Application.DataTransferObject.Responses;
using Financii.Application.DataTransferObject.Responses.Budget;
using Financii.Application.Interfaces.AppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    [Authorize]
    public class BudgetController : BaseController
    {
        private readonly IBudgetPlanAppService _budgetPlanAppService;

        public BudgetController(IBudgetPlanAppService budgetPlanAppService)
        {
            _budgetPlanAppService = budgetPlanAppService;
        }

        // GET Api/v1/Budget/Current
        [HttpGet]
        public async Task<ActionResult<ApiResponse<CurrentBudgetResponse>>> Current()
            => HandleResult(await _budgetPlanAppService.GetCurrentAsync(GetUserId()));

        // POST Api/v1/Budget/Create
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CurrentBudgetResponse>>> Create(
            [FromBody] CreateBudgetRequest request)
        {
            var result = await _budgetPlanAppService.CreateAsync(request, GetUserId());

            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e.Message == "BUDGET_ALREADY_EXISTS"))
                    return Conflict(new { code = "BUDGET_ALREADY_EXISTS" });

                var errors = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(ApiResponse<CurrentBudgetResponse>.Failure("Operation not allowed.", errors: errors));
            }

            return StatusCode(201, ApiResponse<CurrentBudgetResponse>.Success(result.Value, statusCode: 201));
        }

        // POST Api/v1/Budget/{budgetPublicId}/Items
        [HttpPost("{budgetPublicId}/Items")]
        public async Task<ActionResult<ApiResponse<BudgetItemMutationResponse>>> AddItem(
            [FromRoute] Guid budgetPublicId,
            [FromBody] BudgetItemPayloadRequest request)
        {
            var result = await _budgetPlanAppService.AddItemAsync(budgetPublicId, request, GetUserId());

            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e.Message == "BUDGET_ACCESS_DENIED"))
                    return StatusCode(403, new { code = "BUDGET_ACCESS_DENIED" });

                var errors = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(ApiResponse<BudgetItemMutationResponse>.Failure("Operation not allowed.", errors: errors));
            }

            return StatusCode(201, ApiResponse<BudgetItemMutationResponse>.Success(result.Value, statusCode: 201));
        }

        // PUT Api/v1/Budget/{budgetPublicId}/Items/{itemPublicId}
        [HttpPut("{budgetPublicId}/Items/{itemPublicId}")]
        public async Task<ActionResult<ApiResponse<BudgetItemMutationResponse>>> UpdateItem(
            [FromRoute] Guid budgetPublicId,
            [FromRoute] Guid itemPublicId,
            [FromBody] BudgetItemPayloadRequest request)
        {
            var result = await _budgetPlanAppService.UpdateItemAsync(budgetPublicId, itemPublicId, request, GetUserId());

            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e.Message == "BUDGET_ACCESS_DENIED"))
                    return StatusCode(403, new { code = "BUDGET_ACCESS_DENIED" });

                var errors = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(ApiResponse<BudgetItemMutationResponse>.Failure("Operation not allowed.", errors: errors));
            }

            return HandleResult(result);
        }

        // DELETE Api/v1/Budget/{budgetPublicId}/Items/{itemPublicId}
        [HttpDelete("{budgetPublicId}/Items/{itemPublicId}")]
        public async Task<ActionResult<ApiResponse<RemoveBudgetItemResponse>>> RemoveItem(
            [FromRoute] Guid budgetPublicId,
            [FromRoute] Guid itemPublicId)
        {
            var result = await _budgetPlanAppService.RemoveItemAsync(budgetPublicId, itemPublicId, GetUserId());

            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e.Message == "BUDGET_ACCESS_DENIED"))
                    return StatusCode(403, new { code = "BUDGET_ACCESS_DENIED" });

                var errors = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(ApiResponse<RemoveBudgetItemResponse>.Failure("Operation not allowed.", errors: errors));
            }

            return HandleResult(result);
        }

        // GET Api/v1/Budget/{budgetPublicId}/Summary
        [HttpGet("{budgetPublicId}/Summary")]
        public async Task<ActionResult<ApiResponse<BudgetSummaryResponse>>> Summary(
            [FromRoute] Guid budgetPublicId)
        {
            var result = await _budgetPlanAppService.GetSummaryAsync(budgetPublicId, GetUserId());

            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e.Message == "BUDGET_ACCESS_DENIED"))
                    return StatusCode(403, new { code = "BUDGET_ACCESS_DENIED" });

                var errors = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(ApiResponse<BudgetSummaryResponse>.Failure("Operation not allowed.", errors: errors));
            }

            return HandleResult(result);
        }
    }
}
