using System.ComponentModel.DataAnnotations;
using RegistroEstudiante.Application.Common;

namespace RegistroEstudiante.Application.Roles;

public class RoleRequest
{
    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public void Validate()
        => ValidationUtilities.ValidateObject(this);
}
