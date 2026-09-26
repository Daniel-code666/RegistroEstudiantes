using System.ComponentModel.DataAnnotations;
using RegistroEstudiante.Application.Common;

namespace RegistroEstudiante.Application.Users.Dtos;

public class UserFilter : Pagination
{
    [StringLength(100)]
    public string? Search { get; set; }

    [StringLength(50)]
    public string? Role { get; set; }

    public bool? Active { get; set; }

    public new void Validate()
    {
        base.Validate();
        ValidationUtilities.ValidateObject(this);
    }
}
