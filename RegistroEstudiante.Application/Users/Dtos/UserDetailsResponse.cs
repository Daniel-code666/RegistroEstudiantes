using RegistroEstudiante.Domain.Enums;

namespace RegistroEstudiante.Application.Users.Dtos;

public class UserDetailsResponse : UserResponse
{
    public TipoIdentificacion IdentificationType { get; set; }
    public string IdentificationNumber { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
