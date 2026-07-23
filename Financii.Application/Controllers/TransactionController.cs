using Financii.Application.DataTransferObject.Requests.Transaction;
using Financii.Application.DataTransferObject.Responses;
using Financii.Application.DataTransferObject.Responses.Transaction;
using Financii.Application.Interfaces.AppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    [Authorize]
    public class TransactionController : BaseController
    {
        private readonly ITransactionAppService _appService;

        public TransactionController(ITransactionAppService appService)
        {
            _appService = appService;
        }

        // POST Api/v1/Transaction/Create
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TransactionResponse>>> Create(
            [FromBody] CreateTransactionRequest request)
        {
            var result = await _appService.CreateAsync(request, GetUserId());

            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e.Message == "BUDGET_ITEM_ACCESS_DENIED"))
                    return UnprocessableEntity(new { code = "BUDGET_ITEM_ACCESS_DENIED" });

                var errors = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(ApiResponse<TransactionResponse>.Failure("Operation not allowed.", errors: errors));
            }

            return StatusCode(201, ApiResponse<TransactionResponse>.Success(result.Value, statusCode: 201));
        }

        // GET Api/v1/Transaction/List
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedTransactionsResponse>>> List(
            [FromQuery] ListTransactionsQuery query)
        {
            var result = await _appService.ListAsync(query, GetUserId());

            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e.Message == "PERSON_ACCESS_DENIED"))
                    return StatusCode(403, new { code = "PERSON_ACCESS_DENIED" });

                var errors = result.Errors.Select(e => e.Message).ToList();
                return BadRequest(ApiResponse<PagedTransactionsResponse>.Failure("Operation not allowed.", errors: errors));
            }

            return HandleResult(result);
        }
    }
}
