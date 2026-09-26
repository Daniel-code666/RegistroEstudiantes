using System.ComponentModel.DataAnnotations;

namespace RegistroEstudiante.Application.Users.Dtos;

public class RegisterRequest : UserFieldsRequest
{
    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
