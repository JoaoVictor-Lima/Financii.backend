namespace Financii.Application.DataTransferObject.Responses.User
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserResponse User { get; set; } = null!;
    }
}
