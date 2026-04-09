using Financii.Application.DataTransferObject.Responses;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    [ApiController]
    [Route("Api/v1/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        protected long GetUserId()
        {
            var claim = User.FindFirst("id")?.Value;
            if (!long.TryParse(claim, out var userId))
                throw new Exception("Usuário inválido.");
            return userId;
        }

        protected ActionResult<ApiResponse<T>> HandleResult<T>(Result<T> result, int successStatusCode = 200)
        {
            if (result.IsSuccess)
                return Ok(ApiResponse<T>.Success(result.Value, statusCode: successStatusCode));

            var errors = result.Errors.Select(e => e.Message).ToList();
            return BadRequest(ApiResponse<T>.Failure("Operação não permitida.", errors: errors));
        }

        protected ActionResult<ApiResponse<object>> HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok(ApiResponse<object>.Success(null));

            var errors = result.Errors.Select(e => e.Message).ToList();
            return BadRequest(ApiResponse<object>.Failure("Operação não permitida.", errors: errors));
        }
    }
}
