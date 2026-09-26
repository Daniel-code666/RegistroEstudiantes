using RegistroEstudiante.Application.Users.Dtos;

namespace RegistroEstudiante.Application.Authentication.Dtos
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public UserResponse User { get; set; } = new UserResponse();
    }
}
