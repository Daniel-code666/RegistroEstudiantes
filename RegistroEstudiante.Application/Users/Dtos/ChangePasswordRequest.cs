using System.ComponentModel.DataAnnotations;
using RegistroEstudiante.Application.Common;

namespace RegistroEstudiante.Application.Users.Dtos;

public class ChangePasswordRequest
{
    [Required, StringLength(128)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;

    public void Validate()
        => ValidationUtilities.ValidateObject(this);
}
