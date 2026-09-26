using System.ComponentModel.DataAnnotations;

namespace RegistroEstudiante.Application.Users.Dtos;

public class CreateUserRequest : RegisterRequest
{
    [Required, StringLength(50)]
    public string Role { get; set; } = string.Empty;
}
