using System.ComponentModel.DataAnnotations;
using RegistroEstudiante.Application.Common;

namespace RegistroEstudiante.Application.Subjects.Dtos;

public class SubjectFilter : Pagination
{
    public bool? Assigned { get; set; }

    [StringLength(100)]
    public string? Search { get; set; }

    [Range(1, int.MaxValue)]
    public int? ProfessorId { get; set; }

    public bool? Active { get; set; } = true;

    public new void Validate()
    {
        base.Validate();
        ValidationUtilities.ValidateObject(this);
    }
}
