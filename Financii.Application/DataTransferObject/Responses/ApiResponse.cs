namespace Financii.Application.DataTransferObject.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccessful { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Success(T data, string message = "Operação realizada com sucesso.", int statusCode = 200)
            => new() { IsSuccessful = true, StatusCode = statusCode, Message = message, Data = data };

        public static ApiResponse<T> Failure(string message, int statusCode = 400, List<string> errors = null)
            => new() { IsSuccessful = false, StatusCode = statusCode, Message = message, Errors = errors ?? new() };
    }
}
