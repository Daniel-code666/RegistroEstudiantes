using RegistroEstudiante.Application.Users.Dtos;

namespace RegistroEstudianteBack.Authentication;

public class BootstrapAdminOptions : CreateUserRequest
{
    public bool Enabled { get; set; }
}
