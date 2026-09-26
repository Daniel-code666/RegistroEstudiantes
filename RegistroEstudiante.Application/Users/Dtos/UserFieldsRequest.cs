using System.ComponentModel.DataAnnotations;
using RegistroEstudiante.Application.Common;
using RegistroEstudiante.Domain.Enums;

namespace RegistroEstudiante.Application.Users.Dtos;

public class UserFieldsRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [EnumDataType(typeof(TipoIdentificacion))]
    public TipoIdentificacion IdentificationType { get; set; }

    [Required, StringLength(30)]
    public string IdentificationNumber { get; set; } = string.Empty;

    public void Validate()
        => ValidationUtilities.ValidateObject(this);
}
